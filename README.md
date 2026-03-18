# TaskFlow DevOps Setup

Bu repoya tam deploy iskeleti eklendi:

- Docker ile local container orchestration
- Kubernetes raw manifestleri
- Helm chart
- GitHub Actions ile CI, container publish ve Helm deploy workflow'ları

## 1. Local Docker

```bash
docker compose up --build
```

Uygulama uçları:

- MVC: `http://localhost:8081`
- API: `http://localhost:8080`

Kalıcı SQLite verisi `taskflow-data` volume'unda tutulur.

## 2. Kubernetes Manifestleri

Raw manifestler `k8s/base` altında bulunuyor.

Önce secret dosyasını düzenle:

```bash
cp k8s/base/secret.example.yaml k8s/base/secret.yaml
```

Sonra `jwt-key`, image isimleri ve ingress domain değerlerini güncelle.

Apply:

```bash
kubectl apply -k k8s/base
```

## 3. Helm Deploy

Chart yolu:

```bash
helm/taskflow
```

Örnek kurulum:

```bash
helm upgrade --install taskflow helm/taskflow \
  --namespace taskflow \
  --create-namespace \
  --set api.image.repository=ghcr.io/<owner>/taskflow-api \
  --set api.image.tag=latest \
  --set mvc.image.repository=ghcr.io/<owner>/taskflow-mvc \
  --set mvc.image.tag=latest \
  --set ingress.hosts[0].host=taskflow.example.com \
  --set secret.jwtKey='<strong-secret>'
```

## 4. GitHub Actions

Workflow'lar:

- `ci.yml`: restore, build, publish, Helm lint, manifest validation
- `container-release.yml`: iki image'ı da GHCR'a basar
- `helm-deploy.yml`: manual dispatch ile cluster'a Helm deploy yapar

GitHub tarafında aşağıdaki değerleri tanımla:

- `secrets.KUBE_CONFIG_DATA`: base64 encode edilmiş kubeconfig
- `secrets.JWT_KEY`: production JWT key
- `vars.INGRESS_HOST`: yayın domain'i

GHCR image hedefleri:

- `ghcr.io/<owner>/taskflow-api`
- `ghcr.io/<owner>/taskflow-mvc`

## 5. Runtime Konfigürasyonu

Önemli environment variable'lar:

- `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `ApiBaseUrl`
- `UseHttpsRedirection`

Health endpoint'leri:

- API: `/healthz`, `/readyz`
- MVC: `/healthz`, `/readyz`
