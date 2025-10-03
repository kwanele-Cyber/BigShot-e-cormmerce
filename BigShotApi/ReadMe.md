# BigShot eCommerce API

**Admin API Key:** `ef1a74e8-5fdd-48d5-9439-4386d60bd522`  
**User API Key:** `2fb5c9f8-c7dd-48bf-b4f3-d89c4fa557fa`  

---

## Database Models

# BigShot eCommerce Database Entities

This document outlines the database entities used in the BigShot eCommerce application, including fields, constraints, and relationships.

---

## AppRole
Represents user roles in the system (e.g., Admin, Customer).

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id    | int  | PK          | Role identifier |
| Name  | string | Required, MaxLength(50) | Role name (Admin / Customer) |
| Users | List<AppUser> | - | Users assigned to this role (1:N relationship) |

---

## AppUser
Represents a user in the system.

| Field    | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id       | int  | PK          | User identifier |
| UserName | string | Required, MaxLength(150) | Username |
| Email    | string | Required, EmailAddress | User email |
| ApiKey   | string | Required | API key (auto-generated for authentication) |
| RoleId   | int  | FK -> AppRole | Foreign key referencing user role |
| Role     | AppRole | Required | Navigation property to role |
| Orders   | List<Order> | - | Orders placed by this user (1:N relationship) |

---

## Product
Represents a product that can be purchased.

| Field                   | Type   | Constraints | Description |
|-------------------------|--------|-------------|-------------|
| Id                      | int    | PK, Identity | Product identifier |
| Name                    | string | Required, MaxLength(250) | Product name |
| ImageUrl                | string | Url, MaxLength(500) | URL of the product image |
| ShortDescription        | string | Required, MaxLength(500) | Short plain-text description |
| LongDescriptionMarkdown | string | Required | Markdown-formatted long description |
| LongDescriptionHtml     | string | - | Rendered HTML version of the long description |
| Price                   | double | Range(0.01, ∞) | Product price |
| InStock                 | int    | Range(0, ∞) | Quantity available in stock |
| Rating                  | double | Range(0.0, 5.0) | Product rating (0–5) |

---

## Order
Represents a customer's order.

| Field        | Type   | Constraints | Description |
|--------------|--------|-------------|-------------|
| OrderId      | int    | PK          | Order identifier |
| CustomerName | string | Required    | Customer's name |
| Address      | string | Required    | Delivery address |
| OrderDate    | DateTime | Default: UTC Now | Order creation date |
| Total        | decimal | -          | Total order amount |
| UserId       | int    | FK -> AppUser | Customer who placed the order |
| User         | AppUser | Required   | Navigation property to user |
| Items        | List<OrderItem> | - | List of items in this order (1:N relationship) |

---

## OrderItem
Represents an individual item within an order.

| Field           | Type   | Constraints | Description |
|-----------------|--------|-------------|-------------|
| OrderItemId     | int    | PK, Identity | Order item identifier |
| OrderId         | int    | FK -> Order | Parent order |
| ProductId       | int    | FK -> Product | Product being ordered |
| Quantity        | int    | Required    | Number of units ordered |
| PriceAtPurchase | decimal | Required   | Product price at time of purchase |
| Product         | Product | Optional   | Navigation property to product |
| Order           | Order   | Optional   | Navigation property to order |

---

## Relationships Overview
- **AppRole → AppUser:** One role can have many users (1:N)
- **AppUser → Order:** One user can place many orders (1:N)
- **Order → OrderItem:** One order can contain many order items (1:N)
- **OrderItem → Product:** Each order item references one product (N:1)

---

### Notes
- `AppUser.ApiKey` is automatically generated for API authentication.
- `Product.LongDescriptionMarkdown` stores the original Markdown content; `LongDescriptionHtml` stores the rendered HTML version for display.
- All constraints are enforced via data annotations in C# (e.g., `[Required]`, `[MaxLength]`, `[Range]`).


---

## Endpoints

### Chatbot
- **POST** `/api/Chatbot/recommendations`  
  **Roles:** Admin, Customer  
  **Body:** `ChatbotRequestDto`  
  **Response:** `ChatbotResponseDto`

### Orders
- **GET** `/api/Orders/all`  
  **Roles:** Admin  
  **Query Params:** `pageSize`, `pageIndex`  
- **GET** `/api/Orders/my-orders`  
  **Roles:** Admin, Customer  
  **Query Params:** `pageSize`, `pageIndex`  
- **POST** `/api/Orders`  
  **Roles:** Customer  
  **Body:** `CreateOrderDto`  
- **DELETE** `/api/Orders/{id}`  
  **Roles:** Admin  

### Products
- **GET** `/api/Products/list`  
  **Roles:** Admin, Customer  
- **GET** `/api/Products/{id}`  
  **Roles:** Admin, Customer  
- **POST** `/api/Products`  
  **Roles:** Admin  
  **Body:** `CreateProductDto`  
- **PUT** `/api/Products/{id}`  
  **Roles:** Admin  
  **Body:** `UpdateProductDto`  
- **DELETE** `/api/Products/{id}`  
  **Roles:** Admin  
- **GET** `/api/Products/search`  
  **Roles:** Admin, Customer  
- **GET** `/api/Products/filter`  
  **Roles:** Admin, Customer  
- **GET** `/api/Products/instock`  
  **Roles:** Admin, Customer  
- **GET** `/api/Products/toprated`  
  **Roles:** Admin, Customer  

### Users
- **GET** `/api/Users/me`  
  **Roles:** Admin, Customer  
- **POST** `/api/Users`  
  **Roles:** Admin  
  **Body:** `RegisterUserDto`  
- **GET** `/api/Users`  
  **Roles:** Admin  
- **PUT** `/api/Users/{id}/role`  
  **Roles:** Admin  
- **DELETE** `/api/Users/{id}`  
  **Roles:** Admin  

---

## Security
All endpoints require an **API Key** in the request header:

---

# API Key Authentication & Role-Based Authorization in BigShot

## 1. API Key Authentication

**Middleware:** `ApiKeyMiddleware`

- **Purpose:** Validate incoming HTTP requests for a valid API key.
- **Header used:** `X-Api-Key`
- **How it works:**
  1. Checks if the request contains the `X-Api-Key` header:
     ```csharp
     if (!context.Request.Headers.TryGetValue(APIKEY_HEADER, out var extractedApiKey))
     ```
     - If missing → responds with `401 Unauthorized`.
  2. Looks up the user in the database, including their role:
     ```csharp
     var user = await db.Users.Include(u => u.Role)
                              .FirstOrDefaultAsync(u => u.ApiKey == extractedApiKey.ToString());
     ```
     - If not found → responds with `403 Forbidden`.
  3. Attaches the authenticated user to `HttpContext.Items["User"]`:
     ```csharp
     context.Items["User"] = user;
     ```
     - Allows controllers and filters to access the authenticated user.

- **Key Points:**
  - Only requests with a valid API key can reach protected endpoints.
  - Middleware runs **before the controller action**, ensuring all downstream code has access to the authenticated user.

---

## 2. Role-Based Authorization

### a) `[AuthorizeRole("Role1", "Role2")]`

- **Purpose:** Restrict access to one or more roles.
- **Implementation:** Implements `IAsyncActionFilter`.
- **Mechanism:**
  1. Retrieves the user from `HttpContext`:
     ```csharp
     var user = context.HttpContext.GetCurrentUser();
     ```
  2. Checks if the user exists and their role matches allowed roles:
     ```csharp
     if (user == null || !_roles.Contains(user.Role.Name))
         context.Result = new ForbidResult();
     ```
  3. If role matches → request continues to the controller action.

- **Usage Examples:**
  ```csharp
  [AuthorizeRole("Customer")]
  public async Task<IActionResult> AddOrder(...)

  [AuthorizeRole("Admin", "Customer")]
  public async Task<IActionResult> ListMyOrders(...)
````

### b) `[RequireRole("Role")]`

* **Purpose:** Restrict access to exactly one role.
* **Mechanism:** Uses `HttpContext.Items["User"]` set by `ApiKeyMiddleware`.
* **Check:**

  ```csharp
  if (!string.Equals(user.Role.Name, _role, StringComparison.OrdinalIgnoreCase))
      context.Result = new ForbidResult();
  ```

---

## 3. Integration Flow

1. **Incoming Request:** Client sends a request with `X-Api-Key`.
2. **Middleware:** `ApiKeyMiddleware` validates API key and fetches user + role.
3. **Controller Action:** `[AuthorizeRole]` or `[RequireRole]` checks user role.
4. **Outcome:**

   * ✅ Valid API key + allowed role → request proceeds.
   * ❌ Invalid API key → `401 Unauthorized` or `403 Forbidden`.
   * ❌ Role not allowed → `403 Forbidden`.

---

### Summary Table

| Layer              | Responsibility                 | Failure Result  |
| ------------------ | ------------------------------ | --------------- |
| `ApiKeyMiddleware` | Validate API key and load user | `401/403`       |
| `[AuthorizeRole]`  | Allow multiple roles           | `403 Forbidden` |
| `[RequireRole]`    | Allow exactly one role         | `403 Forbidden` |




