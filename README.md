# gpg-api
An API written in C# that allows users to both encrypt and decrypt text-strings using the GPG encryption method. This repository also conatins a client written in bash that allows users to easily interact with the API.

# Dependencies

* .NET 8 (Required by the API)
* GnuPG (Required by the API)
* Bash (Required by the client)
* cURL (Required by the client)

# How to run API
## 1. Download the project:
```bash
git clone https://github.com/NilsDavid01/gpg-api.git
```
## 2. Navigate into the API folder:
```bash
cd gpg-api/GpgApi
```
## 3. Build and run the API:
```bash
dotnet build && dotnet run
```

# How to run the bash client
## 1. Naigate into the root folder of the project:
```bash
cd gpg-api
```
## 2. Give the bash client execution permission: 
```bash
sudo chmod +x gpg-client.sh
```
## 3. Run the bash client script:
```bash
./gpg-client.sh
```
