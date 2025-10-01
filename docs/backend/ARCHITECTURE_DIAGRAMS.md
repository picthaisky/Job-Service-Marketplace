# System Architecture Diagram

## High-Level Architecture

```mermaid
graph TB
    subgraph "Client Layer"
        A[Web Browser]
        B[Mobile App]
    end
    
    subgraph "API Gateway"
        C[ASP.NET Core API]
        D[Swagger/OpenAPI]
        E[JWT Auth Middleware]
    end
    
    subgraph "Application Layer"
        F[Controllers]
        G[Services]
        H[DTOs]
        I[Business Logic]
    end
    
    subgraph "Domain Layer"
        J[Entities]
        K[Enums]
        L[Domain Rules]
    end
    
    subgraph "Infrastructure Layer"
        M[DbContext]
        N[Repositories]
        O[External Services]
    end
    
    subgraph "Data Layer"
        P[(PostgreSQL Database)]
    end
    
    subgraph "External Services"
        Q[Payment Gateway]
        R[Email Service]
        S[File Storage]
    end
    
    A --> C
    B --> C
    C --> D
    C --> E
    E --> F
    F --> G
    G --> H
    G --> I
    I --> J
    I --> K
    I --> L
    G --> M
    M --> N
    N --> P
    G --> O
    O --> Q
    O --> R
    O --> S
```

---

## Clean Architecture Layers

```mermaid
graph LR
    subgraph "Presentation"
        A[Controllers]
        B[DTOs]
    end
    
    subgraph "Application"
        C[Services]
        D[Business Logic]
        E[Interfaces]
    end
    
    subgraph "Domain"
        F[Entities]
        G[Value Objects]
        H[Domain Events]
    end
    
    subgraph "Infrastructure"
        I[DbContext]
        J[Repositories]
        K[External APIs]
    end
    
    A --> C
    B --> C
    C --> D
    D --> E
    C --> F
    F --> G
    F --> H
    C --> I
    I --> J
    I --> K
```

---

## Payment Flow Sequence

```mermaid
sequenceDiagram
    participant C as Client
    participant A as API
    participant S as Service
    participant DB as Database
    participant PG as Payment Gateway
    
    C->>A: POST /api/bookings
    A->>S: CreateBooking(dto)
    S->>DB: Save Booking
    DB-->>S: Booking Created
    S-->>A: BookingDto
    A-->>C: 201 Created
    
    C->>A: POST /api/payments
    A->>PG: Process Payment
    PG-->>A: Payment Success
    A->>S: ProcessPayment(dto)
    S->>S: Calculate Commission & Tax
    S->>DB: Create Payment Record
    S->>DB: Create Transactions
    DB-->>S: Payment Saved
    S-->>A: PaymentDto
    A-->>C: 201 Created
    
    Note over S,DB: Booking Completed
    
    C->>A: POST /api/bookings/{id}/complete
    A->>S: CompleteBooking(id)
    S->>S: Release Payment
    S->>DB: Update Payment Status
    S->>DB: Create Release Transaction
    S->>DB: Generate Tax Documents
    S->>DB: Update Income Summary
    DB-->>S: All Updates Saved
    S-->>A: Success
    A-->>C: 200 OK
```

---

## Database Entity Relationships

```mermaid
erDiagram
    User ||--o| ProviderProfile : has
    User ||--o{ Booking : "creates (client)"
    User ||--o{ Booking : "accepts (provider)"
    
    ProviderProfile ||--o{ Availability : has
    ProviderProfile ||--o{ Portfolio : has
    
    Booking ||--|| Payment : has
    Booking ||--o| Review : has
    
    Payment ||--o{ Transaction : has
    
    User ||--o{ ProviderIncomeSummary : has
    User ||--o{ TaxDocument : has
    Booking ||--o{ TaxDocument : generates
```

---

## Authentication Flow

```mermaid
sequenceDiagram
    participant U as User
    participant A as API
    participant AS as AuthService
    participant DB as Database
    participant JWT as JWT Service
    
    U->>A: POST /api/auth/register
    A->>AS: Register(dto)
    AS->>DB: Check Email Exists
    DB-->>AS: Email Available
    AS->>AS: Hash Password
    AS->>DB: Create User
    DB-->>AS: User Created
    AS->>JWT: Generate Token
    JWT-->>AS: JWT Token
    AS-->>A: AuthResponse + Token
    A-->>U: 201 Created + Token
    
    Note over U,A: Future Requests
    
    U->>A: GET /api/providers (with Bearer Token)
    A->>JWT: Validate Token
    JWT-->>A: Valid User
    A->>A: Check Authorization
    A->>DB: Get Providers
    DB-->>A: Provider List
    A-->>U: 200 OK + Data
```

---

## Commission Calculation Flow

```mermaid
graph TD
    A[Booking Completed] --> B{Calculate Total Amount}
    B --> C[Gross Amount = HourlyRate × Hours]
    C --> D[Commission = Gross × 10%]
    D --> E[Withholding Tax = Gross × 3%]
    E --> F[Net Amount = Gross - Commission - Tax]
    F --> G[Create Payment Record]
    G --> H[Create Transaction: Payment]
    H --> I[Create Transaction: Commission]
    I --> J[Create Transaction: Withholding Tax]
    J --> K[Hold in Escrow]
    K --> L{Provider Completes Work}
    L -->|Yes| M[Release Net Amount]
    L -->|No| K
    M --> N[Create Transaction: Release]
    N --> O[Generate Tax Documents]
    O --> P[Update Income Summary]
    P --> Q[Complete]
```

---

## Booking Status Lifecycle

```mermaid
stateDiagram-v2
    [*] --> Pending: Client Creates Booking
    Pending --> Accepted: Provider Accepts
    Pending --> Cancelled: Client/Provider Cancels
    Accepted --> InProgress: Work Starts
    Accepted --> Cancelled: Client/Provider Cancels
    InProgress --> Completed: Work Finished
    InProgress --> Disputed: Issue Raised
    Disputed --> Completed: Admin Resolves
    Disputed --> Cancelled: Admin Cancels
    Completed --> [*]: Payment Released
    Cancelled --> [*]: Refund Processed
```

---

## API Request/Response Flow

```mermaid
graph LR
    A[HTTP Request] --> B[Middleware Pipeline]
    B --> C[Authentication]
    C --> D[Authorization]
    D --> E[Routing]
    E --> F[Controller Action]
    F --> G[Model Binding]
    G --> H[Validation]
    H --> I[Service Layer]
    I --> J[Business Logic]
    J --> K[Repository]
    K --> L[Database]
    L --> K
    K --> J
    J --> I
    I --> F
    F --> M[Result Serialization]
    M --> N[HTTP Response]
```

---

## Technology Stack Overview

```mermaid
graph TB
    subgraph "Frontend (Planned)"
        A[Angular 20]
        B[Tailwind CSS]
        C[Material UI]
    end
    
    subgraph "API Layer"
        D[.NET 9]
        E[ASP.NET Core]
        F[Swagger]
    end
    
    subgraph "Authentication"
        G[JWT Bearer]
        H[Identity]
    end
    
    subgraph "ORM & Database"
        I[Entity Framework Core]
        J[Npgsql]
        K[PostgreSQL 15+]
    end
    
    subgraph "External Services"
        L[Payment Gateway]
        M[Email Service]
        N[Cloud Storage]
    end
    
    A --> E
    B --> A
    C --> A
    E --> D
    E --> F
    E --> G
    G --> H
    E --> I
    I --> J
    J --> K
    E --> L
    E --> M
    E --> N
```

---

## Deployment Architecture (Planned)

```mermaid
graph TB
    subgraph "Load Balancer"
        A[NGINX]
    end
    
    subgraph "Web Servers"
        B[API Instance 1]
        C[API Instance 2]
        D[API Instance 3]
    end
    
    subgraph "Database Cluster"
        E[(PostgreSQL Master)]
        F[(PostgreSQL Replica 1)]
        G[(PostgreSQL Replica 2)]
    end
    
    subgraph "Cache Layer"
        H[Redis]
    end
    
    subgraph "Storage"
        I[File Storage S3]
    end
    
    subgraph "Monitoring"
        J[Logging]
        K[Metrics]
        L[Alerts]
    end
    
    A --> B
    A --> C
    A --> D
    B --> E
    C --> E
    D --> E
    E --> F
    E --> G
    B --> H
    C --> H
    D --> H
    B --> I
    C --> I
    D --> I
    B --> J
    C --> J
    D --> J
    J --> K
    K --> L
```

---

## Security Layers

```mermaid
graph TD
    A[Client Request] --> B[HTTPS/TLS]
    B --> C[CORS Policy]
    C --> D[Rate Limiting]
    D --> E[JWT Validation]
    E --> F[Role Authorization]
    F --> G[Input Validation]
    G --> H[SQL Injection Prevention]
    H --> I[XSS Protection]
    I --> J[CSRF Protection]
    J --> K[API Endpoint]
    K --> L[Database Access]
    L --> M[Encrypted Data]
```

---

## Data Flow: Booking to Payment

```mermaid
flowchart LR
    A[Client Creates Booking] --> B[Booking Saved]
    B --> C[Provider Accepts]
    C --> D[Client Makes Payment]
    D --> E[Payment in Escrow]
    E --> F[Work in Progress]
    F --> G[Provider Completes]
    G --> H[System Calculates]
    H --> I[Commission Deducted]
    I --> J[Tax Deducted]
    J --> K[Net Amount Released]
    K --> L[Documents Generated]
    L --> M[Income Summary Updated]
    M --> N[Transaction Complete]
```

---

## Module Dependencies

```mermaid
graph TD
    A[API] --> B[Application]
    A --> C[Infrastructure]
    B --> D[Domain]
    C --> D
    
    B --> E[DTOs]
    B --> F[Services]
    B --> G[Interfaces]
    
    C --> H[DbContext]
    C --> I[Repositories]
    C --> J[External Services]
    
    D --> K[Entities]
    D --> L[Value Objects]
    D --> M[Business Rules]
```

---

## Key Performance Indicators

```mermaid
graph LR
    A[System Metrics] --> B[Response Time]
    A --> C[Throughput]
    A --> D[Error Rate]
    A --> E[Database Performance]
    
    B --> F[< 200ms Average]
    C --> G[1000+ req/sec]
    D --> H[< 1% Error Rate]
    E --> I[Query Time < 50ms]
    
    J[Business Metrics] --> K[Bookings/Day]
    J --> L[Revenue/Month]
    J --> M[Active Users]
    J --> N[Provider Satisfaction]
    
    K --> O[Target: 1000+]
    L --> P[Target: 1M+]
    M --> Q[Target: 10K+]
    N --> R[Target: 4.5+ stars]
```

---

## API Gateway Pattern

```mermaid
graph TB
    subgraph "Client Applications"
        A[Web App]
        B[Mobile App]
        C[Admin Portal]
    end
    
    subgraph "API Gateway"
        D[ASP.NET Core API]
        E[Authentication]
        F[Rate Limiting]
        G[Logging]
    end
    
    subgraph "Microservices (Future)"
        H[User Service]
        I[Booking Service]
        J[Payment Service]
        K[Notification Service]
    end
    
    A --> D
    B --> D
    C --> D
    D --> E
    E --> F
    F --> G
    G --> H
    G --> I
    G --> J
    G --> K
```

---

## Caching Strategy (Planned)

```mermaid
graph TB
    A[API Request] --> B{Check Cache}
    B -->|Hit| C[Return Cached Data]
    B -->|Miss| D[Query Database]
    D --> E[Store in Cache]
    E --> F[Return Data]
    
    G[Cache Invalidation] --> H[On Update]
    G --> I[On Delete]
    G --> J[TTL Expiration]
    
    H --> K[Clear Related Cache]
    I --> K
    J --> K
```

---

## Error Handling Flow

```mermaid
graph TD
    A[Request] --> B[Try Execute]
    B --> C{Success?}
    C -->|Yes| D[Return 200/201]
    C -->|No| E{Error Type}
    E -->|Validation| F[Return 400]
    E -->|Not Found| G[Return 404]
    E -->|Unauthorized| H[Return 401]
    E -->|Forbidden| I[Return 403]
    E -->|Server Error| J[Log Error]
    J --> K[Return 500]
    
    F --> L[Error Response]
    G --> L
    H --> L
    I --> L
    K --> L
```
