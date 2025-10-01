# API Endpoints Specification

## Base URL
```
https://api.jobservicemarketplace.com/api
```

---

## Authentication

### Register
**POST** `/auth/register`

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+66812345678",
  "role": 1
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "role": 1,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### Login
**POST** `/auth/login`

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "role": 1,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

## Users

### Get User Profile
**GET** `/users/{id}`

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "id": 1,
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+66812345678",
  "role": 1,
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

---

### Update User Profile
**PUT** `/users/{id}`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+66812345678"
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+66812345678",
  "role": 1
}
```

---

## Provider Profiles

### Get All Providers
**GET** `/providers`

**Query Parameters:**
- `profession` (optional): Filter by profession
- `location` (optional): Filter by location
- `minRating` (optional): Filter by minimum rating
- `page` (default: 1): Page number
- `pageSize` (default: 10): Items per page

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "userId": 2,
      "profession": "Nurse",
      "bio": "Experienced nurse with 10 years...",
      "skills": "Patient Care, Emergency Response",
      "hourlyRate": 500.00,
      "location": "Bangkok",
      "profileImageUrl": "https://...",
      "isVerified": true,
      "averageRating": 4.8,
      "totalReviews": 25
    }
  ],
  "totalCount": 100,
  "page": 1,
  "pageSize": 10
}
```

---

### Get Provider by ID
**GET** `/providers/{id}`

**Response (200 OK):**
```json
{
  "id": 1,
  "userId": 2,
  "user": {
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane@example.com"
  },
  "profession": "Nurse",
  "bio": "Experienced nurse with 10 years...",
  "skills": "Patient Care, Emergency Response",
  "hourlyRate": 500.00,
  "location": "Bangkok",
  "profileImageUrl": "https://...",
  "isVerified": true,
  "averageRating": 4.8,
  "totalReviews": 25,
  "availabilities": [
    {
      "dayOfWeek": 1,
      "startTime": "08:00:00",
      "endTime": "17:00:00",
      "isAvailable": true
    }
  ],
  "portfolios": [
    {
      "id": 1,
      "title": "Home Care Project",
      "description": "Provided care for elderly patient",
      "imageUrl": "https://..."
    }
  ]
}
```

---

### Create Provider Profile
**POST** `/providers`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "profession": "Nurse",
  "bio": "Experienced nurse with 10 years...",
  "skills": "Patient Care, Emergency Response",
  "hourlyRate": 500.00,
  "location": "Bangkok",
  "certificationDocuments": ["https://...", "https://..."]
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "userId": 2,
  "profession": "Nurse",
  "hourlyRate": 500.00,
  "isVerified": false
}
```

---

### Update Provider Profile
**PUT** `/providers/{id}`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "bio": "Updated bio...",
  "hourlyRate": 550.00,
  "location": "Bangkok, Sukhumvit"
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "bio": "Updated bio...",
  "hourlyRate": 550.00,
  "location": "Bangkok, Sukhumvit"
}
```

---

## Availability

### Create Availability
**POST** `/providers/{providerId}/availabilities`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "dayOfWeek": 1,
  "startTime": "08:00:00",
  "endTime": "17:00:00",
  "isAvailable": true
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "providerProfileId": 1,
  "dayOfWeek": 1,
  "startTime": "08:00:00",
  "endTime": "17:00:00"
}
```

---

### Update Availability
**PUT** `/providers/{providerId}/availabilities/{id}`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "startTime": "09:00:00",
  "endTime": "18:00:00"
}
```

---

## Portfolio

### Add Portfolio Item
**POST** `/providers/{providerId}/portfolios`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "title": "Home Care Project",
  "description": "Provided care for elderly patient",
  "imageUrl": "https://..."
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "title": "Home Care Project",
  "description": "Provided care for elderly patient",
  "imageUrl": "https://..."
}
```

---

## Bookings

### Create Booking
**POST** `/bookings`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "providerId": 2,
  "jobTitle": "Home Nursing Care",
  "jobDescription": "Need nursing care for elderly parent",
  "scheduledStartDate": "2024-02-01T08:00:00Z",
  "scheduledEndDate": "2024-02-01T17:00:00Z",
  "hourlyRate": 500.00,
  "estimatedHours": 8
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "clientId": 1,
  "providerId": 2,
  "jobTitle": "Home Nursing Care",
  "totalAmount": 4000.00,
  "status": 1,
  "createdAt": "2024-01-15T10:00:00Z"
}
```

---

### Get All Bookings
**GET** `/bookings`

**Headers:** `Authorization: Bearer {token}`

**Query Parameters:**
- `status` (optional): Filter by status
- `role` (optional): "client" or "provider"
- `page` (default: 1)
- `pageSize` (default: 10)

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "clientId": 1,
      "providerId": 2,
      "jobTitle": "Home Nursing Care",
      "totalAmount": 4000.00,
      "status": 2,
      "scheduledStartDate": "2024-02-01T08:00:00Z",
      "scheduledEndDate": "2024-02-01T17:00:00Z"
    }
  ],
  "totalCount": 50,
  "page": 1,
  "pageSize": 10
}
```

---

### Get Booking by ID
**GET** `/bookings/{id}`

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "id": 1,
  "client": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe"
  },
  "provider": {
    "id": 2,
    "firstName": "Jane",
    "lastName": "Smith"
  },
  "jobTitle": "Home Nursing Care",
  "jobDescription": "Need nursing care for elderly parent",
  "scheduledStartDate": "2024-02-01T08:00:00Z",
  "scheduledEndDate": "2024-02-01T17:00:00Z",
  "hourlyRate": 500.00,
  "estimatedHours": 8,
  "totalAmount": 4000.00,
  "status": 2,
  "acceptedAt": "2024-01-16T09:00:00Z",
  "createdAt": "2024-01-15T10:00:00Z"
}
```

---

### Accept Booking
**POST** `/bookings/{id}/accept`

**Headers:** `Authorization: Bearer {token}` (Provider only)

**Response (200 OK):**
```json
{
  "id": 1,
  "status": 2,
  "acceptedAt": "2024-01-16T09:00:00Z"
}
```

---

### Complete Booking
**POST** `/bookings/{id}/complete`

**Headers:** `Authorization: Bearer {token}` (Provider only)

**Response (200 OK):**
```json
{
  "id": 1,
  "status": 4,
  "completedAt": "2024-02-01T17:30:00Z"
}
```

---

### Cancel Booking
**POST** `/bookings/{id}/cancel`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "cancellationReason": "Client changed plans"
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "status": 5,
  "cancelledAt": "2024-01-20T10:00:00Z",
  "cancellationReason": "Client changed plans"
}
```

---

## Payments

### Process Payment
**POST** `/payments`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "bookingId": 1,
  "amount": 4000.00,
  "paymentMethod": 1,
  "paymentGateway": "Stripe",
  "paymentGatewayTransactionId": "tx_abc123"
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "bookingId": 1,
  "amount": 4000.00,
  "commissionAmount": 400.00,
  "withholdingTaxAmount": 120.00,
  "netAmount": 3480.00,
  "status": 2,
  "paidAt": "2024-01-15T10:30:00Z"
}
```

---

### Get Payment Details
**GET** `/payments/{id}`

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "id": 1,
  "bookingId": 1,
  "amount": 4000.00,
  "commissionAmount": 400.00,
  "withholdingTaxAmount": 120.00,
  "netAmount": 3480.00,
  "status": 4,
  "paymentMethod": 1,
  "paidAt": "2024-01-15T10:30:00Z",
  "releasedToProviderAt": "2024-02-01T18:00:00Z",
  "transactions": [
    {
      "id": 1,
      "type": 1,
      "amount": 4000.00,
      "description": "Payment received from client"
    },
    {
      "id": 2,
      "type": 2,
      "amount": 400.00,
      "description": "Platform commission (10%)"
    },
    {
      "id": 3,
      "type": 3,
      "amount": 120.00,
      "description": "Withholding tax 3%"
    },
    {
      "id": 4,
      "type": 4,
      "amount": 3480.00,
      "description": "Payment released to provider"
    }
  ]
}
```

---

## Reviews

### Create Review
**POST** `/reviews`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "bookingId": 1,
  "rating": 5,
  "comment": "Excellent service! Very professional."
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "bookingId": 1,
  "reviewerId": 1,
  "revieweeId": 2,
  "rating": 5,
  "comment": "Excellent service! Very professional.",
  "createdAt": "2024-02-02T10:00:00Z"
}
```

---

### Get Provider Reviews
**GET** `/providers/{providerId}/reviews`

**Query Parameters:**
- `page` (default: 1)
- `pageSize` (default: 10)

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "reviewer": {
        "firstName": "John",
        "lastName": "Doe"
      },
      "rating": 5,
      "comment": "Excellent service!",
      "createdAt": "2024-02-02T10:00:00Z"
    }
  ],
  "averageRating": 4.8,
  "totalCount": 25,
  "page": 1,
  "pageSize": 10
}
```

---

## Provider Income

### Get Income Summary
**GET** `/providers/{providerId}/income/summary`

**Headers:** `Authorization: Bearer {token}` (Provider or Admin only)

**Query Parameters:**
- `year` (required): Year for the report

**Response (200 OK):**
```json
{
  "providerId": 2,
  "year": 2024,
  "totalGrossIncome": 250000.00,
  "totalCommission": 25000.00,
  "totalWithholdingTax": 7500.00,
  "totalNetIncome": 217500.00,
  "totalCompletedBookings": 50,
  "updatedAt": "2024-12-31T23:59:59Z"
}
```

---

### Get Monthly Income Breakdown
**GET** `/providers/{providerId}/income/monthly`

**Headers:** `Authorization: Bearer {token}` (Provider or Admin only)

**Query Parameters:**
- `year` (required): Year
- `month` (optional): Specific month (1-12)

**Response (200 OK):**
```json
{
  "providerId": 2,
  "year": 2024,
  "month": 1,
  "grossIncome": 20000.00,
  "commission": 2000.00,
  "withholdingTax": 600.00,
  "netIncome": 17400.00,
  "completedBookings": 4
}
```

---

## Tax Documents

### Get Tax Documents
**GET** `/providers/{providerId}/tax-documents`

**Headers:** `Authorization: Bearer {token}` (Provider or Admin only)

**Query Parameters:**
- `year` (required): Year
- `documentType` (optional): 1=PND3, 2=Invoice, 3=Receipt

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "documentType": 1,
      "documentNumber": "PND3-2024-001",
      "documentUrl": "https://storage.../pnd3-001.pdf",
      "year": 2024,
      "amount": 120.00,
      "issuedDate": "2024-02-01T00:00:00Z",
      "booking": {
        "id": 1,
        "jobTitle": "Home Nursing Care"
      }
    }
  ],
  "totalCount": 50
}
```

---

### Download Tax Document
**GET** `/tax-documents/{id}/download`

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
- Content-Type: application/pdf
- Content-Disposition: attachment; filename="PND3-2024-001.pdf"

---

## Admin Endpoints

### Get All Users (Admin)
**GET** `/admin/users`

**Headers:** `Authorization: Bearer {token}` (Admin only)

**Query Parameters:**
- `role` (optional): Filter by role
- `isActive` (optional): Filter by active status
- `page`, `pageSize`

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "role": 1,
      "isActive": true
    }
  ],
  "totalCount": 1000,
  "page": 1,
  "pageSize": 20
}
```

---

### Verify Provider (Admin)
**POST** `/admin/providers/{id}/verify`

**Headers:** `Authorization: Bearer {token}` (Admin only)

**Response (200 OK):**
```json
{
  "id": 1,
  "isVerified": true
}
```

---

### Platform Analytics (Admin)
**GET** `/admin/analytics/dashboard`

**Headers:** `Authorization: Bearer {token}` (Admin only)

**Query Parameters:**
- `startDate` (optional)
- `endDate` (optional)

**Response (200 OK):**
```json
{
  "totalUsers": 5000,
  "totalProviders": 1200,
  "totalClients": 3800,
  "totalBookings": 10000,
  "completedBookings": 8500,
  "totalRevenue": 5000000.00,
  "totalCommission": 500000.00,
  "totalWithholdingTax": 150000.00,
  "averageBookingValue": 4500.00,
  "bookingsByStatus": {
    "pending": 500,
    "accepted": 300,
    "inProgress": 200,
    "completed": 8500,
    "cancelled": 400,
    "disputed": 100
  }
}
```

---

## Error Responses

### 400 Bad Request
```json
{
  "error": "Bad Request",
  "message": "Invalid input data",
  "errors": {
    "email": ["Email is required", "Email format is invalid"],
    "password": ["Password must be at least 8 characters"]
  }
}
```

### 401 Unauthorized
```json
{
  "error": "Unauthorized",
  "message": "Invalid or expired token"
}
```

### 403 Forbidden
```json
{
  "error": "Forbidden",
  "message": "You don't have permission to access this resource"
}
```

### 404 Not Found
```json
{
  "error": "Not Found",
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "error": "Internal Server Error",
  "message": "An unexpected error occurred"
}
```

---

## Rate Limiting

All API endpoints are rate-limited:
- **Anonymous users**: 100 requests per hour
- **Authenticated users**: 1000 requests per hour
- **Admin users**: 5000 requests per hour

Rate limit headers:
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 950
X-RateLimit-Reset: 1704067200
```

---

## Pagination

All list endpoints support pagination:
- Default `pageSize`: 10
- Maximum `pageSize`: 100

Response format:
```json
{
  "data": [...],
  "totalCount": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10
}
```
