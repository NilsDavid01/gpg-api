#!/usr/bin/env bash

CONFIG_FILE="$HOME/.gpg-api-client"
API_URL="http://localhost:5034"

[[ -f "$CONFIG_FILE" ]] && source "$CONFIG_FILE"

save_config() {
  echo "API_URL=$API_URL" > "$CONFIG_FILE"
}

check_api() {
  curl -s "$API_URL/api/gpg" >/dev/null && echo "API reachable" || echo "API not reachable"
}

encrypt() {
	read -p "Recipient key: " recipient
  read -p "Message: " message

  json=$(jq -n \
    --arg text "$message" \
    --arg recipient "$recipient" \
    '{ text: $text, recipient: $recipient }')

  curl -s -X POST "$API_URL/api/gpg/encrypt" \
    -H "Content-Type: application/json" \
    -d "$json" | jq -r .encrypted
}

decrypt() {
	echo "Paste encrypted text (Ctrl+D when done):"
  encrypted=$(cat)

  json=$(jq -n --arg text "$encrypted" '{ text: $text }')

  curl -s -X POST "$API_URL/api/gpg/decrypt" \
    -H "Content-Type: application/json" \
    -d "$json" | jq -r .decrypted
}

generate_key() {
  read -p "Name: " name
  read -p "Email: " email

  json=$(jq -n --arg name "$name" --arg email "$email" \
    '{ name: $name, email: $email }')

  curl -s -X POST "$API_URL/api/gpg/keys/generate" \
    -H "Content-Type: application/json" \
    -d "$json"
}

list_keys() {
  curl -s "$API_URL/api/gpg/keys" | jq -r .keys
}

import_key() {
	echo "Paste GPG key (Ctrl+D when done):"
  KEY_DATA=$(cat)

  curl -s -X POST "$API_URL/api/gpg/keys/import" \
    -H "Content-Type: text/plain" \
    --data-binary "$KEY_DATA"
}

export_key() {

 read -p "Key ID or email to export: " KEY_ID

  OUTPUT_FILE="exported-key.asc"

  HTTP_CODE=$(curl -s -w "%{http_code}" \
    -o "$OUTPUT_FILE" \
    "$API_URL/api/gpg/keys/export?keyId=$KEY_ID")

  if [ "$HTTP_CODE" != "200" ]; then
    echo "Export failed (HTTP $HTTP_CODE)"
    cat "$OUTPUT_FILE"
    rm -f "$OUTPUT_FILE"
    return
  fi

  echo "Key exported successfully to $OUTPUT_FILE"
  echo "-----"
  cat "$OUTPUT_FILE"

}

set_api() {
  read -p "API URL: " API_URL
  save_config
}

menu() {
  echo "1) Encrypt message"
  echo "2) Decrypt message"
  echo "3) Generate GPG key"
  echo "4) List GPG keys"
  echo "5) Import GPG key"
  echo "6) Export GPG key"
  echo "7) Set API URL"
  echo "8) Check API connection"
  echo "9) Exit"
}

while true; do
  menu
  read -p "> " choice
  case $choice in
    1) encrypt ;;
    2) decrypt ;;
    3) generate_key ;;
    4) list_keys ;;
    5) import_key ;;
    6) export_key ;;
    7) set_api ;;
    8) check_api ;;
    9) exit ;;
  esac
done
