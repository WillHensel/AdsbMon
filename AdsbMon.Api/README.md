
## Building and running the Docker image

Use the following command from inside the `AdsbMon.Api` directory:
```bash
docker build -t <image_tag> -f ./Dockerfile ../ 
```

To then run the image, making it accessible at port 8000 on the host machine:
```bash
docker run --rm -it -p 8000:8080 -e ASPNETCORE_HTTP_PORTS=8080 <image_tag>
```