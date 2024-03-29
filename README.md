# Gulla.EpiserverCensorFaces for CMS 12

This is the readme for the CMS 12 version, the version for CMS 11 is [over here](https://github.com/tomahg/Gulla.Episerver.CensorFaces/blob/cms11/README.md).

***Warning:*** This addon is using `System.Drawing`, and because of that it currently only works on Windows, not Linux.

## Censors eye region of faces
This addon will use Azure Cognitive Services Face API to detect faces, and place a black rectangle over the eye region of all detected faces, in images uploaded in Optimizely Content Cloud (formerly Episerver CMS).

![Censored](images/censored.jpg)

## Prerequisites
* Create an Azure Cognitive Services, Face API-resource using the Azure portal.
* Add the following keys to appsettings.json. Get the values from the Azure portal.
``` 
  "Gulla": {
    "EpiserverCensorFaces": {
      "CognitiveServices": {
        "SubscriptionKey": "my-subscription-key",
        "Endpoint": "https://my-service.cognitiveservices.azure.com/"
      } 
    } 
  } 
```
  
## More information
Check out [this blog post](https://www.gulla.net/no/blog/episerver-image-anonymization-using-microsoft-cognitive-services-and-face-api/) or [this blog post](https://www.gulla.net/no/blog/personvernvennlig-bildeopplasting/).

## Get it
Grab it from this repository or install the nuget available on nuget.org as [Gulla.EpiserverCensorFaces](https://www.nuget.org/packages/Gulla.Episerver.CensorFaces/). Check «Include prerelease» in Visual Studio.
