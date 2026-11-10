# FaceAuth API

Multi-tenant .NET 9 face authentication Web API for developers. Clients capture images; the API detects faces, stores embeddings, and verifies identity with optional passcodes.

## Quick start (Docker)

```bash
# from repo root
docker compose -f docker/docker-compose.yml --profile dev up --build
```

- API: http://localhost:8080  
- Swagger: http://localhost:8080/swagger  
- Health: http://localhost:8080/health  
- Adminer (dev profile): http://localhost:8081  
- Default admin key: `dev-admin-key` (override with `FACEAUTH_ADMIN_KEY`)

Production (no Adminer):

```bash
set FACEAUTH_ADMIN_KEY=your-strong-admin-key
docker compose -f docker/docker-compose.yml up --build -d
```

## Developer flow

1. **Provision tenant** (admin key):

```bash
curl -X POST http://localhost:8080/v1/admin/tenants ^
  -H "X-Admin-Key: dev-admin-key" ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Acme App\"}"
```

2. **Issue API token** (shown once):

```bash
curl -X POST http://localhost:8080/v1/admin/tenants/{tenantId}/tokens ^
  -H "X-Admin-Key: dev-admin-key" ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"prod\",\"scopes\":[\"users:write\",\"users:read\",\"faces:enroll\",\"faces:verify\",\"faces:delete\"]}"
```

3. **Create user / enroll / verify** with `X-Api-Key: fa_live_...`:

```bash
curl -X POST http://localhost:8080/v1/users ^
  -H "X-Api-Key: YOUR_TOKEN" ^
  -H "Content-Type: application/json" ^
  -d "{\"userName\":\"alice\",\"passcode\":\"1234\"}"

curl -X POST http://localhost:8080/v1/users/{userId}/faces ^
  -H "X-Api-Key: YOUR_TOKEN" ^
  -F "file=@face.jpg"

curl -X POST http://localhost:8080/v1/auth/verify ^
  -H "X-Api-Key: YOUR_TOKEN" ^
  -F "file=@face.jpg" ^
  -F "passcode=1234"
```

## Auth

| Header | Purpose |
|--------|---------|
| `X-Api-Key` | Tenant developer token (`fa_live_{prefix}_{secret}`) |
| `Authorization: Bearer ...` | Same as API key |
| `X-Admin-Key` | Platform admin (tenants, tokens, usage) |

Tokens are stored hashed. Raw token is returned only at creation time.

## Local run (without Docker API)

```bash
docker compose -f docker/docker-compose.yml up db -d
dotnet run --project src/FaceAuth.Api
```

EF migrations apply automatically on startup.
