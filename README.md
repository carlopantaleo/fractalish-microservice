# Fractalish Microservice

A 10-hour challenge by YanchWare.

## Abstract

This microservice implements a simple Cloud Service Broker exposing an Open Service Broker API (OSB API) compliant
interface. It allows provisioning and managing of virtual machines (VMs) using Amazon Web Services (AWS) as the
underlying infrastructure provider, however it is designed to be capable of being easily extended by implementing the 
appropriate classes for other cloud providers.

## Design Choices

- **OSB API Compliance**: The API surface tries to strictly adhere to the OSB API specification. Since this is a 
  10-hour challenge, only a small part of the specification is implemented.
- **Layered Architecture**:
    - **API**: Exposes RESTful endpoints according to OSB API spec using ASP.NET controllers.
    - **Abstractions**: Defines common interfaces for different cloud providers and
      exceptions (`IVmInstanceService`, `InvalidConfigurationException`, `ServiceInstanceException`).
    - **Infrastructure**: Provides implementations of OSB-related services (like `OsbService`) and general classes 
      used by the microservice, like an `ExceptionHandlerMiddleware`.
    - **Specific implementations**: Provides infrastructure-specific logic for interfacing with cloud provider APIs 
      (`AwsVmInstanceService`).
- **Dependency Injection**: Leverages the .NET Core DI framework to decouple components, improving testability and 
  making the microservice composable by design.
- **AWS Integration**: Employs the AWS SDK for .NET to interact with the EC2 service for managing VM instances.
- **Error handling**: Minimal error handling is implemented for Provisioning and Fetching at the AWS implementation 
  level. This choice has been made to demonstrate some logic of error translation between provider-specific (AWS) 
  errors and platform-generic errors.
- **Unit tests**: the whole project has been implemented in TDD: this results in a nearly-100% coverage.

## Technologies Used

- **.NET 8**: Backend framework
- **ASP.NET Core**: Framework for building web APIs
- **Swagger/OpenAPI**:  API documentation (access `/swagger` endpoint once running)
- **AWS SDK for .NET**: AWS API integration
- **Moq**: Unit testing library
- **FluentAssertions**: Assertion library to improve readability of tests
- **AutoFixture**:  Library to simplify the creation of test data

## Assumptions

- The service is stateless, relying on passed identifiers like `instanceId`. In a production-ready system, this would be
  backed by persistent storage to manage instance state and mapping between internal resource identifiers and the
  service instance ID provided by the OSB client.
- Only a subset of the OSB API is implemented (Provision, Deprovision, Fetch).
- Only AWS is supported as a backing infrastructure provider.
- Minimal error handling
- The service catalog reads available services from `appsettings.json`, but in a real-world application a more
  elaborated implementation (such as a database-backed catalog) should be considered.
- No integration tests or live tests were performed. I don't have a billed AWS account available... :)
- No logging has been implemented. Obviously logging is an important part of a production-ready application, but for a 
  10-hour challenge I didn't want to spend time including it.
- Authentication is not implemented for this demo but should be considered for production (API key, OAuth2, etc.).

## How to Run the Microservice

### In a local environment

1. **Prerequisites**:
    - .NET 8 SDK
    - AWS Credentials: Properly configure access key, secret key and region either through environment variables, or in
      the `appsettings.json` file in the `AwsConfiguration` section:
      ```json
      {
        "AwsConfiguration": {
            "AccessKey": "your-access-key",
            "SecretKey": "your-secret-key",
            "Region": "your-region"
         }
      }
      ```
2. **Building the Project**:
    - `dotnet build`
3. **Running the Microservice**:
    - `dotnet run --launch-profile https --project FractalishMicroservice.App`

### In a Docker container

1. **Prerequisites**:
    - Docker
    - AWS Credentials: Properly passed to the container through environment variables:
      ```
      AwsConfiguration__AccessKey="your-access-key"
      AwsConfiguration__SecretKey="your-access-key"
      AwsConfiguration__Region="your-region"
      ```
2. **Building the Project**:
   In the root directory of the solution, run
   - `docker build -f FractalishMicroservice.App/Dockerfile .`
3. **Running the Microservice**:
    - Run the just-created container image and make sure to pass the AwsConfiguration environment variables.

## Interacting with the API

**Base URL:**  `https://localhost:7048/v2/` (make sure to allow insecure connections in your browser if running it
locally), or whichever port you have mapped if running in a Docker container.

### Authentication

Authentication is **not implemented** for this demo but should be considered for production.

### Catalog (GET /v2/catalog)

Retrieve the service catalog to determine available service offerings and plans. This should be done by the platform 
(eg: your marketplace) before attempting to create any instance. At the moment, the service catalog reads available 
services from `appsettings.json`, but in a real-world application a more elaborated implementation (such as a 
database-backed catalog) should be considered.

```http
GET /v2/catalog
```

**Example Response:**

```json
{
  "services": [
    {
      "id": "example-service-offering-1",
      "name": "Example Service Offering 1",
      "description": "A very simple offering",
      "bindable": true,
      "plans": [
        {
          "id": "example-plan-1",
          "name": "Basic Plan",
          "description": "Basic service plan with limited resources."
        },
        {
          "id": "example-plan-2",
          "name": "Premium Plan",
          "description": "Premium service plan with more resources."
        }
      ]
    }
  ]
}
```

### Service Instance Provisioning (PUT /v2/service_instances/{instance_id})

**Request Body Parameters:**

- `instance_id`: (path) A GUID identifying the service instance. This is provided by the client.
- `service_id`:  The ID of the service from the catalog (e.g., "example-service-id").
- `plan_id`: The ID of the plan to provision (e.g., "example-plan-id").
- `organization_guid`: An ID representing the owning organization.
- `space_guid`: An ID representing the target space within an organization.

**Example Request:**

```http
PUT /v2/service_instances/your-instance-guid-here 
Content-Type: application/json

{
  "service_id": "example-service-id",
  "plan_id": "example-plan-id",
  "organization_guid": "your-org-guid",
  "space_guid": "your-space-guid" 
}
```

**Response:**

```http
200 OK
Content-Type: application/json

{
  "operation": "your-created-vm-instance-id" // In this demo, this is the created AWS instance ID. In a real-world 
                                             // scenario this should be a platform-generate unique ID representing
                                             // the running asyncronous operation.
}
```

**Remarks**

In this demo, since the catalog is mocked in `appsettings.json`, no checks are performed on `instance_id` and 
`plan_id` and they should correspond to VM images and plans available for the AWS account used.

### Service Instance Deprovisioning (DELETE /v2/service_instances/{instance_id})

Deprovision an instance.

**Request:**

```http
DELETE /v2/service_instances/your-instance-guid-here
```
- `instance_id`: (path) A GUID identifying the service instance.

**Response:**

```http
200 OK
```

**Remarks**

`instance_id` should be the ID provided by the client when provisioning, however in this demo this is the AWS instance
ID.

### Service Instance Fetching (GET /v2/service_instances/{instance_id})

Retrieve the state and other relevant information for a provisioned instance.

**Request:**

```http
GET /v2/service_instances/your-instance-guid-here
```
- `instance_id`: (path) A GUID identifying the service instance. This should be the ID provided by the client when
  provisioning, however in this demo this is the AWS instance ID.

**Response:**

```json
{
  "service_id": "example-service-id",
  "plan_id": "example-plan-id",
  "parameters": {
    "state": "Running"
  }
} 
```

**Remarks**

In this demo, some response data is mocked.
