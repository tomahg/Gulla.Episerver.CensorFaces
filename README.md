# Gulla.EpiserverCensorFaces

## Censors eye region of faces
Will use Azure Cognitive Services Face API to detect faces, and place a black rectangle over the eye region of all detected faces, in images uploaded in Episerver CMS.

![Censored](images/censored.jpg)

## Prerequisites
* Create an Azure Cognitive Services, Face API-resource using the Azure portal.
* Add the following keys to appsettings section in web.config. Get the values from the Azure portal.  
  - Gulla.EpiserverCensorFaces:CognitiveServices.SubscriptionKey
  - Gulla.EpiserverCensorFaces:CognitiveServices.Endpoint
  
## More information
Check out [this blog post](https://blog.novacare.no/episerver-image-anonymization-using-microsoft-cognitive-services-and-face-api/).

## Get it
Grab it from this repository or install the nuget available on nuget.org as [Gulla.EpiserverCensorFaces](https://www.nuget.org/packages/Gulla.EpiserverCensorFaces/). Check «Include prerelease» in Visual Studio.
