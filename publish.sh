#!/bin/bash
set -e

echo "docker build ..."
docker build -t "daniellarsennz/helloaspdotnetcore:latest" -t "daniellarsennz/helloaspdotnetcore:${{ github.sha }}"

echo 'docker login...'
docker login -u daniellarsennz -p ${{ secrets.DOCKER_HUB_PASSWORD }}

echo 'docker push...'
docker push "daniellarsennz/helloaspdotnetcore:latest"
