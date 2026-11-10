namespace FaceAuth.Application.Abstractions;

public interface IFaceRecognitionService
{
    /// <summary>Detects the primary face and returns a normalized embedding, or null if none.</summary>
    Task<float[]?> ExtractEmbeddingAsync(byte[] imageBytes, CancellationToken cancellationToken = default);

    double CosineSimilarity(float[] a, float[] b);
}
