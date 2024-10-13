#!/bin/bash
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