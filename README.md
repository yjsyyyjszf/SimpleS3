# SimpleS3

[![NuGet](https://img.shields.io/nuget/v/Genbox.SimpleS3.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Genbox.SimpleS3/)

### Description
A C# implementation of Amazon's S3 API with a focus on simplicity, security and performance. Download or upload an object with a single line of code.

### Support for S3 features
* Streaming chunked encoding support
* Server-side encryption with customer keys
* Support for path and virtual host style buckets
* Support most S3 functions. See the [S3 API status page](https://github.com/Genbox/SimpleS3/wiki/S3-API-status). Something missing? You can help by [contributing](https://github.com/Genbox/SimpleS3/wiki/Extending-SimpleS3).

### API features
These are the features provided by this API implementation.
* Supports a fluent API to make it easy to download/upload an object
* Uses [HttpClientFactory](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests) for optimal performance and flexibility
* Dependency injection friendly. By default uses [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/)
* Supports configuration binding via [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2)
* Extensive unit tests ensure correctness and stability
* Support for uploading/downloading multiparts in parallel
* Support for third-party servers like [Minio](https://min.io/) and [Wasabi](https://wasabi.com/)
* ProfileManager to securely manage credentials

## Extensions
SimpleS3 is very extensible and has multiple different network drivers and addons you can use. Click on the extension below for more information.

* [SimpleS3.Extensions.HttpClient](https://github.com/Genbox/SimpleS3/tree/master/src/SimpleS3.Extensions.HttpClient)
* [SimpleS3.Extensions.HttpClientFactory](https://github.com/Genbox/SimpleS3/tree/master/src/SimpleS3.Extensions.HttpClientFactory)
* [SimpleS3.Extensions.ProfileManager](https://github.com/Genbox/SimpleS3/tree/master/src/SimpleS3.Extensions.ProfileManager)

## Examples

#### Setup the config and client
```csharp
S3Client client = new S3Client("MyKeyId", "MyAccessKey", AwsRegion.EuWest1)
```

#### Or use Microsoft's Dependency Injection
```csharp
ServiceCollection services = new ServiceCollection();
services.AddSimpleS3(config => {
    config.Credentials = new SecretAccessKey("MyKeyId", "MyAccessKey");
    config.Region = AwsRegion.EuWest1;
});

ServiceProvider provider = services.BuildServiceProvider();
IS3Client client = provider.GetRequiredService<IS3Client>();
```

#### Upload an object to a bucket
```csharp
await client.PutObjectStringAsync("MyBucket", "MyObject", "Hello World!");
```

#### Download an object
```csharp
string content = await client.GetObjectStringAsync("MyBucket", "MyObject");

if (content != null)
    Console.WriteLine(content); //Outputs: Hello World!
else
    Console.WriteLine("Something went wrong");
```

#### Delete an object
```csharp
await client.DeleteObjectAsync("MyBucket", "MyObject");
```

### Fluent API
The fluent API makes downloading/uploading objects easier by providing a convenient way of supplying information such as cache control, content-disposition, encryption keys, etc.
```csharp
//Upload string
Upload upload = client.Transfer
                 .UploadString("MyBucket", "MyObject", "Hello World!", Encoding.UTF8)
                 .WithAccessControl(ObjectCannedAcl.PublicReadWrite)
                 .WithCacheControl(CacheControlType.NoCache)
                 .WithEncryption();

PutObjectResponse resp = await upload.ExecuteAsync();
Console.WriteLine("Success? " + resp.IsSuccess);

//Download string
Download download = client.Transfer
                     .Download("MyBucket", "MyObject")
                     .WithRange(0, 5);                    

GetObjectResponse resp2 = await download.ExecuteAsync();
Console.WriteLine(await resp2.Content.AsStringAsync()); //outputs 'Hello'
```

See the code in SimpleS3.Examples for more examples.
