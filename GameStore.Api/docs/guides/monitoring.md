# Monitoring Guide

Complete guide to monitoring the GameStore API with Prometheus and Grafana.

## Quick Access

| Service | URL | Default Credentials |
|---------|-----|---------------------|
| Grafana | http://localhost:3000 | admin / admin |
| Prometheus | http://localhost:9090 | - |
| API Metrics | http://localhost:8080/metrics | - |

## Getting Started

### 1. Start Monitoring Stack

```bash
docker compose up -d prometheus grafana postgres redis
```

### 2. Verify Services

```bash
# Check Prometheus
curl http://localhost:9090/-/healthy

# Check Grafana
curl http://localhost:3000/api/health

# Check API metrics
curl http://localhost:8080/metrics
```

### 3. Access Grafana Dashboard

1. Open http://localhost:3000
2. Login with `admin` / `admin`
3. Navigate to **Dashboards** → **Browse**
4. Select **GameStore API Overview**

## Metrics Endpoint

The API exposes Prometheus metrics at `/metrics`:

```bash
curl http://localhost:8080/metrics
```

Sample output:
```
# HELP http_server_request_duration_seconds HTTP request duration in seconds
# TYPE http_server_request_duration_seconds histogram
http_server_request_duration_seconds_bucket{endpoint="/games",method="GET",status_code="200",le="0.005"} 123
...

# HELP gamestore_games_created_total Number of games created
# TYPE gamestore_games_created_total counter
gamestore_games_created_total 42
```

## Available Metrics

### HTTP Metrics (Built-in)

| Metric | Type | Description |
|--------|------|-------------|
| `http_server_requests_received_total` | Counter | Total HTTP requests |
| `http_server_requests_failed_total` | Counter | Failed HTTP requests |
| `http_server_request_duration_seconds` | Histogram | Request latency |

### Business Metrics

| Metric | Type | Description |
|--------|------|-------------|
| `gamestore_games_created_total` | Counter | Games created |
| `gamestore_games_updated_total` | Counter | Games updated |
| `gamestore_games_deleted_total` | Counter | Games deleted |
| `gamestore_cache_hits` | Counter | Cache hits |
| `gamestore_cache_misses` | Counter | Cache misses |
| `gamestore_command_duration` | Histogram | Command handler duration (ms) |
| `gamestore_query_handler_duration` | Histogram | Query handler duration (ms) |

## Grafana Dashboard Panels

### 1. Requests Per Second
- Shows incoming request rate by method and route
- Useful for: Capacity planning, traffic monitoring

### 2. Request Latency (p99)
- 99th percentile response time
- Useful for: SLA monitoring, performance tuning

### 3. Error Rate
- Percentage of failed requests
- Useful for: Availability monitoring

### 4. Cache Hit Ratio
- Percentage of cache hits vs misses
- Useful for: Cache efficiency monitoring
- Target: > 80% is healthy

### 5. Command Handler Duration
- Average execution time of write operations
- Useful for: Identifying slow operations

### 6. Query Handler Duration
- Average execution time of read operations
- Useful for: Identifying slow queries

### 7. Games Created/Updated/Deleted
- Business activity counters
- Useful for: Usage analytics

## Prometheus Queries

### Common PromQL Examples

```promql
# Request rate per endpoint
rate(http_server_requests_received_total[5m])

# Error rate
rate(http_server_requests_failed_total[5m]) / rate(http_server_requests_received_total[5m])

# p99 latency
histogram_quantile(0.99, rate(http_server_request_duration_seconds_bucket[5m]))

# Cache hit ratio
sum(rate(gamestore_cache_hits[5m])) / (sum(rate(gamestore_cache_hits[5m])) + sum(rate(gamestore_cache_misses[5m])))

# Average command duration
rate(gamestore_command_duration_sum[5m]) / rate(gamestore_command_duration_count[5m])

# Games created per minute
rate(gamestore_games_created_total[1m]) * 60
```

## Alerting

### Recommended Alert Rules

Create `prometheus/alerts.yml`:

```yaml
groups:
  - name: gamestore
    rules:
      - alert: HighErrorRate
        expr: rate(http_server_requests_failed_total[5m]) / rate(http_server_requests_received_total[5m]) > 0.05
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "High error rate detected"
          description: "Error rate is {{ $value | humanizePercentage }}"

      - alert: HighLatency
        expr: histogram_quantile(0.99, rate(http_server_request_duration_seconds_bucket[5m])) > 0.5
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High latency detected"
          description: "p99 latency is {{ $value | humanizeDuration }}"

      - alert: LowCacheHitRatio
        expr: sum(rate(gamestore_cache_hits[5m])) / (sum(rate(gamestore_cache_hits[5m])) + sum(rate(gamestore_cache_misses[5m]))) < 0.5
        for: 10m
        labels:
          severity: warning
        annotations:
          summary: "Low cache hit ratio"
          description: "Cache hit ratio is {{ $value | humanizePercentage }}"
```

## Local Development

### Run Without Docker

1. Start Prometheus:
```bash
docker run -d --name prometheus -p 9090:9090 \
  -v $(pwd)/prometheus.yml:/etc/prometheus/prometheus.yml \
  prom/prometheus
```

2. Start Grafana:
```bash
docker run -d --name grafana -p 3000:3000 grafana/grafana
```

3. Run API:
```bash
dotnet run --project GameStore.Api
```

## Troubleshooting

### Metrics Not Appearing

1. Check API is running:
```bash
curl http://localhost:8080/metrics
```

2. Check Prometheus target:
- Open http://localhost:9090/targets
- Verify `gamestore-api` is UP

3. Check Docker network:
```bash
docker network ls
docker network inspect dotnet-10-api_default
```

### Grafana Dashboard Empty

1. Verify Prometheus datasource:
- Go to **Configuration** → **Data Sources**
- Click **Prometheus** → **Save & Test**

2. Check time range (top right)
- Set to **Last 15 minutes**

3. Check for errors in console (F12)

## Production Considerations

### 1. Secure Grafana
```yaml
grafana:
  environment:
    - GF_SECURITY_ADMIN_PASSWORD=${GRAFANA_PASSWORD}
    - GF_SECURITY_DISABLE_GRAVATAR=true
    - GF_FEATURE_TOGGLES_ENABLE=publicDashboards
```

### 2. Scale Prometheus
```yaml
prometheus:
  command:
    - '--storage.tsdb.retention.time=30d'
    - '--storage.tsdb.retention.size=10GB'
```

### 3. Add Alertmanager
```yaml
alertmanager:
  image: prom/alertmanager:latest
  ports:
    - "9093:9093"
```

## Next Steps

- [Configuration](./configuration.md) - Environment configuration
- [Docker Deployment](./docker-deployment.md) - Container deployment
- [Troubleshooting](./troubleshooting.md) - Common issues
