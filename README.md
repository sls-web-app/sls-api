# sls-api
SLS - web chess api based on .net 9.0

## Development Commands
```bash
dotnet ef migrations add Update --startup-project . --project ../sls-repos
dotnet ef database update --startup-project . --project ../sls-repos
```

**RUN IN sls-api/sls-api DIRECTORY !!!**

## API Documentation
The API includes comprehensive OpenAPI documentation with:
- **XML Documentation Comments**: All controllers and methods have detailed XML comments
- **ProducesResponseType Attributes**: Explicit response type definitions for better documentation
- **Scalar UI**: Beautiful API documentation interface at `/scalar/v1` when running in development

### Accessing API Documentation
When running in development mode:
- **Scalar UI**: `http://localhost:5263/scalar/v1`
- **OpenAPI JSON**: `http://localhost:5263/openapi/v1.json`

The documentation includes:
- Detailed endpoint descriptions
- Parameter descriptions with examples
- Response type definitions
- HTTP status code explanations
- Request/response schemas