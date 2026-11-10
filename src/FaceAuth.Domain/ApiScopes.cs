namespace FaceAuth.Domain;

public static class ApiScopes
{
    public const string FacesEnroll = "faces:enroll";
    public const string FacesVerify = "faces:verify";
    public const string FacesDelete = "faces:delete";
    public const string UsersRead = "users:read";
    public const string UsersWrite = "users:write";

    public static readonly string[] All =
    [
        FacesEnroll,
        FacesVerify,
        FacesDelete,
        UsersRead,
        UsersWrite
    ];
}
