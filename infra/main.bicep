// Parameters
@description('The location for all resources')
param location string = resourceGroup().location

@description('The name of the App Service')
param appServiceName string = 'namesearch-${uniqueString(resourceGroup().id)}'

@description('The SKU for the App Service Plan')
@allowed([
  'F1'
  'B1'
  'B2'
  'S1'
  'S2'
  'P1V2'
  'P2V2'
])
param appServicePlanSku string = 'B1'

@description('Azure Search Service name')
param searchServiceName string

@description('Azure Search Service endpoint')
param searchServiceEndpoint string

@description('Azure Search Index name')
param searchIndexName string

@description('Azure OpenAI Service endpoint')
param openAIEndpoint string

@description('Azure OpenAI API Key')
@secure()
param openAIApiKey string

@description('Azure OpenAI Deployment Name')
param openAIDeploymentName string = 'gpt-4'

// Variables
var appServicePlanName = '${appServiceName}-plan'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: appServicePlanSku
  }
  properties: {
    reserved: true // Linux
  }
  kind: 'linux'
}

// App Service
resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: appServiceName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      http20Enabled: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'ApiBaseUrl'
          value: 'https://${appServiceName}.azurewebsites.net'
        }
        {
          name: 'AzureSearch__ServiceName'
          value: searchServiceName
        }
        {
          name: 'AzureSearch__Endpoint'
          value: searchServiceEndpoint
        }
        {
          name: 'AzureSearch__IndexName'
          value: searchIndexName
        }
        {
          name: 'OpenAI__Endpoint'
          value: openAIEndpoint
        }
        {
          name: 'OpenAI__ApiKey'
          value: openAIApiKey
        }
        {
          name: 'OpenAI__DeploymentName'
          value: openAIDeploymentName
        }
        {
          name: 'OpenAI__MaxTokens'
          value: '150'
        }
        {
          name: 'OpenAI__Temperature'
          value: '0.7'
        }
      ]
    }
  }
}

// Outputs
output appServiceUrl string = 'https://${appService.properties.defaultHostName}'
output appServiceName string = appService.name
output principalId string = appService.identity.principalId
