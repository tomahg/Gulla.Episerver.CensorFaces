# Gulla.EpiserverCensorFaces for CMS 11

This is the readme for the CMS 11 version, the version for CMS 12 is [over here](https://github.com/tomahg/Gulla.Episerver.CensorFaces/tree/main).

## Censors eye region of faces

This addon will use Azure Cognitive Services Face API to detect faces, and place a black rectangle over the eye region of all detected faces, in images uploaded in Optimizely Content Cloud (formerly Episerver CMS).

![Censored](images/censored.jpg)

## Prerequisites

-   Create an Azure Cognitive Services, Face API-resource using the Azure portal.
-   Add the following keys to appsettings section in web.config. Get the values from the Azure portal.
    -   Gulla.EpiserverCensorFaces:CognitiveServices.SubscriptionKey
    -   Gulla.EpiserverCensorFaces:CognitiveServices.Endpoint

## More information

Check out [this blog post](https://www.gulla.net/no/blog/episerver-image-anonymization-using-microsoft-cognitive-services-and-face-api/).

## Get it

Install the NuGet package [Gulla.Episerver.CensorFaces](https://nuget.optimizely.com/package/?id=Gulla.Episerver.CensorFaces&v=1.0.7-preview.1). You need to check «Include prerelease» in Visual Studio, or your packet manager.
