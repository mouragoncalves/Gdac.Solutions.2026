#!/usr/bin/env bash
# setup-vps.sh — Provisiona o VPS para hospedar auth (prod) e auth-stg (staging)
# Execute como root no servidor KingHost Ubuntu 24.04
# Uso: bash setup-vps.sh <dominio_prod> <dominio_stg> <email_letsencrypt>
# Ex:  bash setup-vps.sh auth.gdac.com.br auth-stg.gdac.com.br suporte@gdac.com.br

set -euo pipefail

DOMAIN_PROD="${1:?Informe o domínio de produção, ex: auth.gdac.com.br}"
DOMAIN_STG="${2:?Informe o domínio de staging, ex: auth-stg.gdac.com.br}"
LETSENCRYPT_EMAIL="${3:?Informe o e-mail para o Let's Encrypt}"
REPO="https://github.com/mouragoncalves/Gdac.Solutions.2026.git"

echo "==> [1/8] Atualizando sistema"
apt-get update -q && apt-get upgrade -y -q

echo "==> [2/8] Instalando dependências base"
apt-get install -y -q curl git nginx certbot python3-certbot-nginx ufw

echo "==> [3/8] Instalando Docker"
if ! command -v docker &>/dev/null; then
  curl -fsSL https://get.docker.com | sh
fi
systemctl enable --now docker

echo "==> [4/8] Configurando firewall (UFW)"
ufw --force enable
ufw allow OpenSSH
ufw allow 'Nginx Full'

echo "==> [5/8] Criando diretórios da aplicação"
mkdir -p /opt/gdac/auth/backups
mkdir -p /opt/gdac/auth-stg

echo "==> [6/8] Clonando repositório"
if [ ! -d /opt/gdac/auth/.git ]; then
  git clone "$REPO" /opt/gdac/auth
fi
if [ ! -d /opt/gdac/auth-stg/.git ]; then
  git clone -b staging "$REPO" /opt/gdac/auth-stg
fi

echo "==> [7/8] Configurando nginx"
cp /opt/gdac/auth/docker/nginx/conf.d/production.conf /etc/nginx/conf.d/auth-prod.conf
cp /opt/gdac/auth/docker/nginx/conf.d/staging.conf    /etc/nginx/conf.d/auth-stg.conf

# Substitui os domínios caso diferentes dos padrões do repo
sed -i "s/auth\.gdac\.com\.br/$DOMAIN_PROD/g"     /etc/nginx/conf.d/auth-prod.conf
sed -i "s/auth-stg\.gdac\.com\.br/$DOMAIN_STG/g"  /etc/nginx/conf.d/auth-stg.conf
sed -i "s/auth\.gdac\.com\.br/$DOMAIN_PROD/g"     /etc/nginx/conf.d/auth-stg.conf  # caminho do cert

# Valida config sem SSL (antes do certbot)
sed -i '/ssl_/d; /listen 443/d; /return 301/d' /etc/nginx/conf.d/auth-prod.conf || true

nginx -t && systemctl restart nginx

echo "==> [8/8] Emitindo certificado SSL (Let's Encrypt — SAN)"
certbot --nginx \
  -d "$DOMAIN_PROD" \
  -d "$DOMAIN_STG" \
  --non-interactive \
  --agree-tos \
  --email "$LETSENCRYPT_EMAIL" \
  --redirect

# Restaura configs nginx originais (com SSL) após certbot
cp /opt/gdac/auth/docker/nginx/conf.d/production.conf /etc/nginx/conf.d/auth-prod.conf
cp /opt/gdac/auth/docker/nginx/conf.d/staging.conf    /etc/nginx/conf.d/auth-stg.conf
sed -i "s/auth\.gdac\.com\.br/$DOMAIN_PROD/g"     /etc/nginx/conf.d/auth-prod.conf
sed -i "s/auth-stg\.gdac\.com\.br/$DOMAIN_STG/g"  /etc/nginx/conf.d/auth-stg.conf
sed -i "s/auth\.gdac\.com\.br/$DOMAIN_PROD/g"     /etc/nginx/conf.d/auth-stg.conf

nginx -t && systemctl reload nginx

echo ""
echo "============================================"
echo "  Servidor provisionado com sucesso!"
echo "============================================"
echo ""
echo "  Proximos passos manuais:"
echo "  1. Gere o par de chaves RSA JWT:"
echo "       openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:4096 -out private.pem"
echo "       openssl rsa -pubout -in private.pem -out public.pem"
echo ""
echo "  2. Crie /opt/gdac/auth/.env (producao)"
echo "     e /opt/gdac/auth-stg/.env (staging)"
echo "     usando docker/docker-compose.yml.example como base."
echo "     As chaves JWT devem estar em linha unica com \\n literal."
echo ""
echo "  3. Crie a chave SSH para deploy automatico:"
echo "       ssh-keygen -t ed25519 -C 'gdac-deploy' -f /root/.ssh/gdac_deploy -N ''"
echo "       cat /root/.ssh/gdac_deploy.pub >> /root/.ssh/authorized_keys"
echo "     Adicione o conteudo de /root/.ssh/gdac_deploy como secret no GitHub."
echo ""
echo "  4. Configure os secrets no GitHub (ver DEPLOY-SETUP.md)."
echo ""
