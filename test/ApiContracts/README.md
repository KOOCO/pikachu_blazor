# API Contract Testing

This directory contains baseline API contracts for preventing breaking changes.

## Files
- `baseline-openapi.json` - The baseline OpenAPI specification that client applications depend on
- `README.md` - This file

## How it works
1. During CI/CD, the current API contract is generated from `/swagger/v1/swagger.json`
2. It's compared with the baseline stored here
3. If endpoints are removed or changed incompatibly, the build can be configured to fail
4. New endpoints are logged but don't break the build

## Updating the baseline
Only update `baseline-openapi.json` when you intentionally want to make breaking changes:
1. Review the breaking changes carefully
2. Ensure all client applications can handle the changes
3. Update the baseline contract
4. Communicate changes to client teams
