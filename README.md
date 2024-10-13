# Pod-til-pod tls demo

## Forberedelser

### Før demo

```
docker pull kindest/node:v1.31.0
helm repo add jetstack https://charts.jetstack.io --force-update

kind create cluster --config tls-demo-kind-config.yaml --image kindest/node:v1.31.0

docker pull quay.io/jetstack/cert-manager-controller:v1.15.3
docker pull quay.io/jetstack/cert-manager-startupapicheck:v1.15.3
docker pull quay.io/jetstack/cert-manager-webhook:v1.15.3
docker pull quay.io/jetstack/cert-manager-cainjector:v1.15.3
docker pull quay.io/jetstack/cert-manager-acmesolver:v1.15.3
docker pull quay.io/jetstack/cert-manager-startupapicheck:v1.15.3
docker pull quay.io/jetstack/cert-manager-package-debian:20210119.0
docker pull quay.io/jetstack/trust-manager:v0.12.0
docker pull registry.k8s.io/ingress-nginx/controller:v1.11.2
docker pull registry.k8s.io/ingress-nginx/kube-webhook-certgen:v1.4.3

kind -n tlsdemo-cluster load docker-image quay.io/jetstack/cert-manager-controller:v1.15.3
kind -n tlsdemo-cluster load docker-image quay.io/jetstack/cert-manager-startupapicheck:v1.15.3
kind -n tlsdemo-cluster load docker-image quay.io/jetstack/cert-manager-webhook:v1.15.3
kind -n tlsdemo-cluster load docker-image quay.io/jetstack/cert-manager-cainjector:v1.15.3
kind -n tlsdemo-cluster load docker-image quay.io/jetstack/cert-manager-acmesolver:v1.15.3
kind -n tlsdemo-cluster load docker-image quay.io/jetstack/cert-manager-package-debian:20210119.0
kind -n tlsdemo-cluster load docker-image quay.io/jetstack/trust-manager:v0.12.0
kind -n tlsdemo-cluster load docker-image registry.k8s.io/ingress-nginx/controller:v1.11.2
kind -n tlsdemo-cluster load docker-image registry.k8s.io/ingress-nginx/kube-webhook-certgen:v1.4.3
docker build -t tls-demo:latest .
kind load docker-image tls-demo:latest -n tlsdemo-cluster
kubectl create ns tls-demo

kubectl apply -n ingress-nginx -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml

```

# Demo

```
helm install \
  cert-manager jetstack/cert-manager \
  --namespace cert-manager \
  --create-namespace \
  --version v1.15.3 \
  --set crds.enabled=true
  
  helm upgrade trust-manager jetstack/trust-manager \
  --install \
  --namespace cert-manager \
  --wait
  ```


Vi må lage en root-CA:

```
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: selfsigned-issuer
spec:
  selfSigned: {}

---

apiVersion: cert-manager.io/v1
kind: Certificate
metadata:
  name: my-selfsigned-ca
  namespace: cert-manager
spec:
  isCA: true
  commonName: my-selfsigned-ca
  secretName: root-secret
  privateKey:
    algorithm: ECDSA
    size: 256
  issuerRef:
    name: selfsigned-issuer
    kind: ClusterIssuer
    group: cert-manager.io

---

apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: my-ca-issuer
spec:
  ca:
    secretName: root-secret


```

# Bygge test-applikasjonen:
```
docker build -t tlstest .
kind load docker-image tlstest -n tlsdemo-cluster
```
