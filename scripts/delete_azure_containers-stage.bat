@echo off
:: Set variables
set STORAGE_ACCOUNT_NAME=stsmoothstage
set RESOURCE_GROUP=rg-smooth-stage-weu

:: Authenticate to Azure
az login 

:: Define the containers
set CONTAINERS=new-media data

:: Loop through each container and delete all files
for %%C in (%CONTAINERS%) do (
    echo Deleting blobs from container: %%C
    az storage blob delete-batch --account-name %STORAGE_ACCOUNT_NAME% --source %%C
)

echo All files in the specified containers have been deleted.

pause
