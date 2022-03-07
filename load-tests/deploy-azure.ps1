$Redis = $false
$ApplicationInsights = $true

$ErrorActionPreference = 'Stop'

$location = 'eastus2'
$loc = 'eus2'
$rg = "helloaspnet-$loc-rg"
$plan = "helloaspnet-$loc-plan"
$tags = 'project=helloaspnet'
$app = "helloaspnet-$loc"
$insights = 'hello-aspnet-insights'
$insightsRG = 'helloaspnet-rg'
$redis = "helloaspnet-$loc-redis"

# Scale settings
$sku = 'P1V3'
$numberWorkers = 2

if ($ResourceGroup -eq $true) {
    # Create resource group
    Write-Host "az group create -n $rg" -ForegroundColor Yellow
    az group create -n $rg --location $location --tags $tags
}

if ($AppService -eq $true) {
    # Create App Service Plan
    Write-Host "az appservice plan create -n $plan" -ForegroundColor Yellow
    az appservice plan create -n $plan -g $rg --location $location --sku $sku --number-of-workers $numberWorkers --is-linux --tags $tags

    # Create App, deploy from Github
    Write-Host "az webapp create -n $app" -ForegroundColor Yellow
    az webapp create -g $rg -p $plan -n $app -i daniellarsennz/helloaspdotnetcore

    # Disable ARR Affinity
    Write-Host "az webapp update -n $app --client-affinity-enabled false" -ForegroundColor Yellow
    az webapp update -n $app -g $rg --client-affinity-enabled false
}

if ($Redis -eq $true) {
    # AZURE CACHE FOR REDIS
    Write-Host "az redis create -n $redis" -ForegroundColor Yellow
    az redis create -l $location -n $redis -g $rg --sku 'Standard' --vm-size 'C1' --redis-version 6
    
    Write-Host "az redis list-keys -n $redis" -ForegroundColor Yellow
    $redisKey = (az redis list-keys -g $rg -n $redis | ConvertFrom-Json ).primaryKey
    
    Write-Host "az webapp config appsettings set -n $app" -ForegroundColor Yellow
    az webapp config appsettings set -n $app -g $rg --settings "AzureCacheRedisConnectionString=$redis.redis.cache.windows.net:6380,password=$redisKey,ssl=True,abortConnect=False"
}

if ($ApplicationInsights -eq $true) {
    #  https://docs.microsoft.com/en-us/cli/azure/ext/application-insights/monitor/app-insights/component?view=azure-cli-latest
    #Write-Host "az monitor app-insights component create --app $insights" -ForegroundColor Yellow
    #$instrumentationKey = ( az monitor app-insights component create --app $insights --location $location -g $rg --tags $tags | ConvertFrom-Json ).instrumentationKey
    
    Write-Host "az monitor app-insights component show --app $insights" -ForegroundColor Yellow
    $instrumentationKey = ( az monitor app-insights component show --app $insights -g $insightsRG | ConvertFrom-Json ).instrumentationKey

    #  https://docs.microsoft.com/en-us/cli/azure/webapp/config/appsettings?view=azure-cli-latest#az-webapp-config-appsettings-set
    Write-Host "az webapp config appsettings set -n $app" -ForegroundColor Yellow
    az webapp config appsettings set -n $app -g $rg --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$instrumentationKey"
}

# Open the apps in the browser
Write-Host "https://$app.azurewebsites.net"

# Tear down
# az group delete -n $rg --yes