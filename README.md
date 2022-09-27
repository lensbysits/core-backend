# backend-core

## Exception handling
We introduced a new way of exception handling by protecting sensitive information to be send to the front-end. This way we prevent a malicious user to gain insights in how the back-end works by reading detailed server errors with stack traces.
A new type of exception is introduced: a `PublicException`. All exception inheriting from this type are send to the front-end (without a stack trace). All of these errors have an error code which can be used for multilingual exceptions in the front-end.
For backwards compatibility we introduced a flag to disable this new type of exception handling, but be aware that you put your API at risk by exposing sensitive information.

The general exception text defaults to _“Unexpected error occurred”_, but can be overridden in the appsettings.json using the following JSON

```
"ApiExceptionHandlingConfig": {
    "DisableEnhancedApiProtection": false|true,
    "GenericErrorMessage": "string",
    "GenericErrorCode": "string"
}
```
