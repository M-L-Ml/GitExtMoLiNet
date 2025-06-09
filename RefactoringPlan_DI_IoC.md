# Refactoring Plan: Transition to Dependency Injection (DI) and Inversion of Control (IoC)

## Objective
Refactor the codebase to use Dependency Injection (DI) and Inversion of Control (IoC) principles, leveraging `Microsoft.Extensions.DependencyInjection` for service registration and resolution. This will improve testability, maintainability, and extensibility of the codebase.

---

## Plan

### **1. Introduce a DI Container**
- Add a reference to `Microsoft.Extensions.DependencyInjection` in all relevant projects.
- Create a central `IServiceCollection` to manage service registrations and build the `IServiceProvider`.

---

### **2. Refactor Service Registration**
- Replace the manual service registration in `GitExtensions.ServiceContainerRegistry.RegisterServices` and similar methods with `IServiceCollection` extensions.
- For each `RegisterServices` method:
  - Convert `serviceContainer.AddService<T>` calls to `services.AddSingleton<T>` or `services.AddTransient<T>` as appropriate.
  - Use extension methods to group service registrations logically (e.g., `AddGitCommandsServices`, `AddGitUIServices`).

---

### **3. Update Service Retrieval**
- Replace all `IServiceProvider.GetService<T>` and `IServiceProvider.GetRequiredService<T>` calls with constructor injection.
- Refactor classes to accept dependencies via constructors instead of retrieving them manually.

---

### **4. Refactor Tests**
- Update test cases to use a mock `IServiceProvider` or `IServiceCollection` for dependency injection.
- Replace manual service mocking with DI-based mocking using libraries like `NSubstitute`.

---

### **5. Update Application Entry Point**
- Modify the `Program.Main` method to:
  - Create an `IServiceCollection`.
  - Register all services using the refactored `RegisterServices` methods.
  - Build the `IServiceProvider` and pass it to the application.

---

### **6. Refactor GlobalServiceContainer**
- Replace `GlobalServiceContainer.CreateDefaultMockServiceContainer` with a method that sets up a mock `IServiceCollection` for tests.
- Use `services.AddSingleton` or `services.AddTransient` to register mock services.

---

### **7. Refactor Specific Areas**
#### **7.1 GitExtensions.ServiceContainerRegistry**
- Replace `serviceContainer.AddService` calls with `services.AddSingleton` or `services.AddTransient`.
- Create extension methods for logical grouping (e.g., `AddGitExtensionsServices`).

#### **7.2 GitCommands.ServiceContainerRegistry**
- Refactor to use `IServiceCollection` for registering `ISubmoduleStatusProvider` and other services.

#### **7.3 GitExtUtils.ServiceContainerRegistry**
- Refactor to use `IServiceCollection` for registering `ISubscribableTraceListener`.

#### **7.4 UI Integration Tests**
- Update `GlobalServiceContainer` to use `IServiceCollection` for mock service registration.

---

### **8. Validate and Test**
- Run all unit and integration tests to ensure the refactor does not break existing functionality.
- Add new tests to validate DI container setup and service resolution.

---

### **9. Documentation**
- Update the documentation to reflect the new DI-based architecture.
- Provide examples of how to register and resolve services using the new approach.

---

### **10. Incremental Refactoring**
- Refactor one area at a time (e.g., `GitExtensions.ServiceContainerRegistry` first, then `GitCommands.ServiceContainerRegistry`, etc.).
- Commit changes incrementally to ensure the codebase remains functional throughout the process.

---

## Key Benefits
- Improved testability through constructor injection.
- Centralized service registration and management.
- Easier to extend and maintain the codebase.