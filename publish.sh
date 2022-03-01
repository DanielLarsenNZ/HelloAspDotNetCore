#!/bin/bash
set -e

echo 'docker login...'
docker login -u daniellarsennz -p ${{ DOCKER_HUB_PASSWORD }}

echo "docker build ..."
docker build . -t "daniellarsennz/helloaspdotnetcore:latest" #-t "daniellarsennz/helloaspdotnetcore:${{ github.sha }}"

echo 'docker push...'
docker push "daniellarsennz/helloaspdotnetcore:latest"
