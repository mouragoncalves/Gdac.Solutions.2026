#!/usr/bin/env bash
# Gera par de chaves RSA 4096 bits para assinatura JWT.
# Uso: ./scripts/generate-keys.sh [diretório destino]

set -euo pipefail

DIR="${1:-secrets}"
mkdir -p "$DIR"

PRIVATE_KEY="$DIR/jwt_private.pem"
PUBLIC_KEY="$DIR/jwt_public.pem"

if [[ -f "$PRIVATE_KEY" ]]; then
  echo "Chave privada já existe em $PRIVATE_KEY. Abortando para não sobrescrever."
  exit 1
fi

echo "Gerando chave privada RSA 4096..."
openssl genrsa -out "$PRIVATE_KEY" 4096 2>/dev/null

echo "Extraindo chave pública..."
openssl rsa -in "$PRIVATE_KEY" -pubout -out "$PUBLIC_KEY" 2>/dev/null

chmod 600 "$PRIVATE_KEY"
chmod 644 "$PUBLIC_KEY"

echo ""
echo "Chaves geradas:"
echo "  Privada : $PRIVATE_KEY"
echo "  Pública : $PUBLIC_KEY"
echo ""
echo "Para configurar via dotnet user-secrets (development):"
echo "  cd Src/Gdac.Auth.Api"
echo "  dotnet user-secrets set \"Jwt:PrivateKey\" \"\$(cat ../../$PRIVATE_KEY)\""
echo "  dotnet user-secrets set \"Jwt:PublicKey\"  \"\$(cat ../../$PUBLIC_KEY)\""
echo ""
echo "ATENÇÃO: nunca comite a chave privada no repositório."
