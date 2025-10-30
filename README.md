# E-Commerce Marten ES Demo

This project is a sample e-commerce application built with .NET, demonstrating the use of Marten for event sourcing and a document database. It follows a clean architecture approach, separating concerns into `Domain`, `Application`, `Infrastructure`, and `API` projects.

## Technologies

*   .NET 8
*   ASP.NET Core
*   Marten (Event Sourcing and Document DB on top of PostgreSQL)
*   MediatR
*   Clean Architecture
*   Domain-Driven Design (DDD)
*   Event Sourcing
*   CQRS

## Getting Started

### Prerequisites

*   .NET 8 SDK
*   Docker (or a running instance of PostgreSQL)

### Installation

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/oskardudycz/ecommerce-marten-es-demo.git
    ```

2.  **Start the database:**

    A `docker-compose.yml` file is provided to easily start a PostgreSQL instance.

    ```bash
    docker-compose up -d
    ```

3.  **Run the application:**

    ```bash
    dotnet run --project src/API/API.csproj
    ```

The API will be available at `http://localhost:5000`. You can access the Swagger UI at `http://localhost:5000/swagger`.

## API Usage

The application provides the following API endpoints:

### Customers

*   `POST /api/customers`: Registers a new customer.

    **Request Body:**

    ```json
    {
      "name": "John Doe",
      "email": "john.doe@example.com"
    }
    ```

### Products

*   `POST /api/products`: Creates a new product.

    **Request Body:**

    ```json
    {
      "name": "Laptop",
      "price": {
        "amount": 1200,
        "currency": "USD"
      }
    }
    ```

### Shopping Cart

*   `POST /api/shopping-carts`: Opens a new shopping cart for a customer.

    **Request Body:**

    ```json
    {
      "customerId": "e4b7b8b0-8b0a-4b1a-9b0a-0b0b0b0b0b0b"
    }
    ```

*   `POST /api/shopping-carts/{shoppingCartId}/products`: Adds a product to the shopping cart.

    **Request Body:**

    ```json
    {
      "productId": "f4b7b8b0-8b0a-4b1a-9b0a-0b0b0b0b0b0b",
      "quantity": 1
    }
    ```

*   `GET /api/shopping-carts/{shoppingCartId}`: Retrieves a summary of the shopping cart.

*   `PUT /api/shopping-carts/{shoppingCartId}/confirm`: Confirms the shopping cart and initiates the checkout process.

## Shopping Cart Checkout Process

The shopping cart checkout process is managed by the `ShoppingCartCheckoutProcessManager`, which is a Marten `ISubscription`. This process manager listens for `ShoppingCartConfirmed` events and orchestrates the creation of an order.

Here's a step-by-step breakdown of the process:

1.  **Confirmation:** When a user confirms their shopping cart via the `PUT /api/shopping-carts/{shoppingCartId}/confirm` endpoint, a `ShoppingCartConfirmed` event is published.

2.  **Event Subscription:** The `ShoppingCartCheckoutProcessManager` is subscribed to this event and is triggered asynchronously.

3.  **Idempotency Check:** The process manager first checks if a `ShoppingCartCheckoutProcess` with the same shopping cart ID already exists. If a process has already been successfully completed, it ignores the event to prevent duplicate order creation.

4.  **Process Creation:** A new `ShoppingCartCheckoutProcess` document is created in the database to track the state of the checkout process.

5.  **Order Creation:** The process manager loads the `ShoppingCartSummary` read model to get the necessary cart details (customer, products, total price). It then sends a `PlaceOrderCommand` to the application layer to create a new order.

6.  **State Update:** Upon successful order creation, the status of the `ShoppingCartCheckoutProcess` is updated to `OrderPlaced`.

7.  **Error Handling:** If any errors occur during the process (e.g., the `ShoppingCartSummary` read model is not yet updated), the process manager logs the error and updates the process status to `Failed`. Marten's built-in retry logic will attempt to re-process the event.

This asynchronous, event-driven approach ensures that the checkout process is resilient and reliable, even in the case of temporary failures.
