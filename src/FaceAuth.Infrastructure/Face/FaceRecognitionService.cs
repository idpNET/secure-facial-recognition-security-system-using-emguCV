using FaceAiSharp;
using FaceAiSharp.Extensions;
using FaceAuth.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FaceAuth.Infrastructure.Face;

public sealed class FaceRecognitionOptions
{
    public string ScrfdModelPath { get; set; } = "onnx/scrfd_2.5g_kps.onnx";
    public string ArcFaceModelPath { get; set; } = "onnx/arcfaceresnet100-11-int8.onnx";
    public float MinDetectionConfidence { get; set; } = 0.5f;
}

public sealed class FaceRecognitionService : IFaceRecognitionService, IDisposable
{
    private readonly FaceRecognitionOptions _options;
    private readonly ILogger<FaceRecognitionService> _logger;
    private readonly object _gate = new();
    private ScrfdDetector? _detector;
    private ArcFaceEmbeddingsGenerator? _embeddings;

    public FaceRecognitionService(IOptions<FaceRecognitionOptions> options, ILogger<FaceRecognitionService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<float[]?> ExtractEmbeddingAsync(byte[] imageBytes, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (_gate)
            {
                EnsureModels();

                using var image = Image.Load<Rgb24>(imageBytes);
                var faces = _detector!.DetectFaces(image)
                    .Where(f => f.Confidence >= _options.MinDetectionConfidence)
                    .OrderByDescending(f => f.Box.Width * f.Box.Height)
                    .ToList();

                if (faces.Count == 0)
                {
                    _logger.LogDebug("No face detected in uploaded image.");
                    return null;
                }

                var face = faces[0];
                if (face.Landmarks is null || face.Landmarks.Count < 5)
                {
                    _logger.LogWarning("Face detected without landmarks; cannot align for ArcFace.");
                    return null;
                }

                var rect = Rectangle.Round(face.Box);
                rect.Intersect(image.Bounds);

                if (rect.Width < 16 || rect.Height < 16)
                    return null;

                using var cropped = image.Clone(ctx => ctx.Crop(rect));

                var localLandmarks = face.Landmarks
                    .Select(p => new PointF(p.X - rect.X, p.Y - rect.Y))
                    .ToList();

                ArcFaceEmbeddingsGenerator.AlignFaceUsingLandmarks(cropped, localLandmarks);
                return _embeddings!.GenerateEmbedding(cropped);
            }
        }, cancellationToken);
    }

    public double CosineSimilarity(float[] a, float[] b) => a.CosineSimilarity(b);

    public void Dispose()
    {
        lock (_gate)
        {
            _detector?.Dispose();
            _embeddings?.Dispose();
            _detector = null;
            _embeddings = null;
        }
    }

    private void EnsureModels()
    {
        if (_detector is not null && _embeddings is not null)
            return;

        var scrfdPath = ResolvePath(_options.ScrfdModelPath);
        var arcPath = ResolvePath(_options.ArcFaceModelPath);

        if (!File.Exists(scrfdPath))
            throw new FileNotFoundException($"SCRFD model not found at '{scrfdPath}'.");
        if (!File.Exists(arcPath))
            throw new FileNotFoundException($"ArcFace model not found at '{arcPath}'.");

        _detector = new ScrfdDetector(new ScrfdDetectorOptions { ModelPath = scrfdPath });
        _embeddings = new ArcFaceEmbeddingsGenerator(new ArcFaceEmbeddingsGeneratorOptions { ModelPath = arcPath });
        _logger.LogInformation("Face recognition models loaded.");
    }

    private static string ResolvePath(string path)
    {
        if (Path.IsPathRooted(path))
            return path;
        return Path.Combine(AppContext.BaseDirectory, path);
    }
}
