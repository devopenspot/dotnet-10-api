# API Reference

Complete documentation for all GameStore API endpoints.

## Base URL

```
http://localhost:8080
```

## Endpoints Overview

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/games` | List all games |
| `GET` | `/games/{id}` | Get game by ID |
| `POST` | `/games` | Create a new game |
| `PUT` | `/games/{id}` | Update a game |
| `DELETE` | `/games/{id}` | Delete a game |

---

## GET /games

Retrieve all games.

### Request

```bash
curl -X GET http://localhost:8080/games
```

### Response

```json
[
  {
    "id": 1,
    "name": "The Legend of Zelda: Breath of the Wild",
    "genre": "Adventure",
    "price": 0.00,
    "releaseDate": "2017-03-03"
  },
  {
    "id": 2,
    "name": "The Witcher 3: Wild Hunt",
    "genre": "RPG",
    "price": 0.00,
    "releaseDate": "2015-05-19"
  }
]
```

### Response Codes

| Code | Description |
|------|-------------|
| `200` | Success |

---

## GET /games/{id}

Retrieve a single game by ID.

### Parameters

| Name | Type | Location | Description |
|------|------|----------|-------------|
| `id` | int | path | Game ID |

### Request

```bash
curl -X GET http://localhost:8080/games/1
```

### Response (Success)

```json
{
  "id": 1,
  "name": "The Legend of Zelda: Breath of the Wild",
  "genre": "Adventure",
  "price": 0.00,
  "releaseDate": "2017-03-03"
}
```

### Response (Not Found)

```json
(null)
```

### Response Codes

| Code | Description |
|------|-------------|
| `200` | Success |
| `404` | Game not found |

---

## POST /games

Create a new game.

### Request Body

```json
{
  "name": "Elden Ring",
  "genreId": 3,
  "price": 59.99,
  "releaseDate": "2022-02-25"
}
```

### Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `name` | string | Yes | Game title |
| `genreId` | int | Yes | Genre ID (1-5) |
| `price` | decimal | Yes | Game price |
| `releaseDate` | string | Yes | Release date (YYYY-MM-DD) |

### Available Genre IDs

| ID | Genre |
|----|-------|
| 1 | Action |
| 2 | Adventure |
| 3 | RPG |
| 4 | Strategy |
| 5 | Simulation |

### Request Example

```bash
curl -X POST http://localhost:8080/games \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Elden Ring",
    "genreId": 3,
    "price": 59.99,
    "releaseDate": "2022-02-25"
  }'
```

### Response

```json
{
  "id": 6,
  "name": "Elden Ring",
  "genre": "RPG",
  "price": 59.99,
  "releaseDate": "2022-02-25"
}
```

### Response Codes

| Code | Description |
|------|-------------|
| `201` | Game created successfully |
| `400` | Invalid request body |

---

## PUT /games/{id}

Update an existing game.

### Parameters

| Name | Type | Location | Description |
|------|------|----------|-------------|
| `id` | int | path | Game ID |

### Request Body

```json
{
  "name": "The Witcher 3: Wild Hunt - Complete Edition",
  "genreId": 3,
  "price": 39.99,
  "releaseDate": "2019-10-15"
}
```

### Request Example

```bash
curl -X PUT http://localhost:8080/games/2 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "The Witcher 3: Wild Hunt - Complete Edition",
    "genreId": 3,
    "price": 39.99,
    "releaseDate": "2019-10-15"
  }'
```

### Response (Success)

```json
{
  "id": 2,
  "name": "The Witcher 3: Wild Hunt - Complete Edition",
  "genre": "RPG",
  "price": 39.99,
  "releaseDate": "2019-10-15"
}
```

### Response Codes

| Code | Description |
|------|-------------|
| `200` | Game updated successfully |
| `404` | Game not found |

---

## DELETE /games/{id}

Delete a game.

### Parameters

| Name | Type | Location | Description |
|------|------|----------|-------------|
| `id` | int | path | Game ID |

### Request Example

```bash
curl -X DELETE http://localhost:8080/games/1
```

### Response Codes

| Code | Description |
|------|-------------|
| `204` | Game deleted successfully |
| `404` | Game not found |

---

## Error Handling

All errors return appropriate HTTP status codes:

| Status | Meaning |
|--------|---------|
| `200` | Success |
| `201` | Created |
| `204` | No Content (Delete success) |
| `400` | Bad Request (Validation error) |
| `404` | Not Found |
| `500` | Internal Server Error |

## Postman Collection

Import this collection for quick testing:

```json
{
  "info": {
    "name": "GameStore API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "List Games",
      "request": {
        "method": "GET",
        "url": "http://localhost:8080/games"
      }
    },
    {
      "name": "Get Game",
      "request": {
        "method": "GET",
        "url": "http://localhost:8080/games/1"
      }
    },
    {
      "name": "Create Game",
      "request": {
        "method": "POST",
        "url": "http://localhost:8080/games",
        "body": {
          "mode": "raw",
          "raw": {
            "name": "New Game",
            "genreId": 1,
            "price": 49.99,
            "releaseDate": "2024-01-01"
          }
        }
      }
    }
  ]
}
```

## Next Steps

- [Architecture Overview](../architecture/README.md) - Understand the codebase
- [Testing Guide](../GameStore.Api.test/docs/README.md) - Test the API
