# API Reference

Base URL: `http://localhost:5167`

## Games

### Get All Games

```http
GET /games
```

**Response 200:**
```json
[
  {
    "id": 1,
    "name": "The Legend of Zelda: Breath of the Wild",
    "genre": "Action",
    "price": 59.99,
    "releaseDate": "2017-03-03"
  }
]
```

---

### Get Game by ID

```http
GET /games/{id}
```

**Parameters:**
| Name | Type | Description |
|------|------|-------------|
| `id` | int | Game ID |

**Response 200:**
```json
{
  "id": 1,
  "name": "The Legend of Zelda: Breath of the Wild",
  "genre": "Action",
  "price": 59.99,
  "releaseDate": "2017-03-03"
}
```

**Response 404:**
```json
{
  "error": "Game not found"
}
```

---

### Create Game

```http
POST /games
Content-Type: application/json

{
  "name": "New Game Title",
  "genreId": 1,
  "price": 59.99,
  "releaseDate": "2024-01-15"
}
```

**Validation Rules:**
- `name`: Required, max 50 characters
- `genreId`: Required, must exist in database
- `price`: Required, must be > 0
- `releaseDate`: Required

**Response 201:** Returns created game with ID

**Response 400:** Validation errors

---

### Update Game

```http
PUT /games/{id}
Content-Type: application/json

{
  "name": "Updated Game Title",
  "genreId": 2,
  "price": 49.99,
  "releaseDate": "2024-06-20"
}
```

**Parameters:**
| Name | Type | Description |
|------|------|-------------|
| `id` | int | Game ID |

**Response 200:** Returns updated game

**Response 404:** Game not found

---

### Delete Game

```http
DELETE /games/{id}
```

**Parameters:**
| Name | Type | Description |
|------|------|-------------|
| `id` | int | Game ID |

**Response 204:** No content (success)

**Response 404:** Game not found

---

## Genres

Genres are managed through game creation. Available genres are seeded on first run.

### List

Genres are returned nested within game responses. To get all genres:

```http
GET /genres
```
