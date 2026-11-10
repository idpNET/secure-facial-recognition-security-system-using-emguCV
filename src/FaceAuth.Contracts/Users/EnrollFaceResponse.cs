namespace FaceAuth.Contracts.Users;

public sealed class EnrollFaceResponse
{
    public Guid FaceSampleId { get; set; }
    public Guid UserId { get; set; }
}
