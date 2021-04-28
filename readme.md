# Create a cluster for development
Run ```new-setup-local-k3x.cmd```.  

## The cluster  

The script will start by craeting the cluster, it will use Ranchers K3d to create a K3s cluster hosted in Docker Desktop. The K3d command will create two containers:  

* The cluster itself, by default it will be named ```k3d-tbilocal-server-0```.  
* A load balancer, by default it will be named ```k3d-tbilocal-serverlb```.     

For the containers a network named ```k3d-tbilocal``` will also be created.

## The registry  

The script will create a Docker registry as the next step, this is needed to be able to push your own images to a location where your cluster could pick them up. The container that will be created for this will be named ```registry.local```.

Next it will connect the ```registry.local``` container to the ```k3d-tbilocal``` network network it will also add a mapped folder to ```registry.local``` where it could save all the images.  

After that it will copy ```registries.yaml``` to the cluster node so ```k3d-tbilocal-server-0```  could find the registry by using http. Normally a cluster node always assumes that registries are located on https url's.  

Instead of copying the file ```registries.yaml``` directly to the cluster node you could shell into the container and run following command to create registries.yaml in the right place:

```
cd /etc/rancher/k3s
touch ./registries.yaml
vi registries.yaml
Press the <i> key to start editing and enter:
mirrors:
  "registry.local:5000":
    endpoint:
    - "http://registry.local:5000"
REM <esc>
REM :wq
```

Manual things to fix afterwards is to edit the file ```%WINDIR%\System32\Drivers\Etc\Hosts``` as administrator and add following line:  

```127.0.0.1 registry.local```  

# Creating registry  

```
REM Edit server-0 in Visual Studio - Containers - Terminal to configure the local registry
REM cd /etc/rancher/k3s
REM touch registries.yaml
REM vi registries.yaml
REM <insert> or <i>
REM mirrors:
REM  "registry.local:5000":
REM     endpoint:
REM       - "http://registry.local:5000"
REM <esc>
REM :wq
```

# Building images

Standing in the $(SolutionRoot) folder you can

# Deploy to kubernetes

Standing in the subfolder $(SolutionRoot)/Deploy  
Run ```kubectl apply -k ./tbilocal``` to deploy all  
Run ```kubectl apply -k ./natsapi``` to deploy only the api  
Run ```kubectl apply -k ./natsconsumer``` to deploy only the consumer  
Run ```kubectl apply -k ./nats``` to deploy only nats jetstream  

Please note that the file ```natsapi/deployment.yaml``` and ```natsconsumer/deployment.yaml``` contains a ```#\{Build.BuildId\}#``` token to be replaced by the build agent, so running locally you need to replace ```#{Build.BuildId}#``` with the string ```"latest"```.  

This solution contains a cmd-file named ```Build.cmd``` and another one named ```Deploy.cmd``` that make use of the application CreateTempAndReplaceTokens and creates a tempfolder where it copies all the deployfiles and replaces ```#{Build.BuildId}#``` with the string ```"latest"```  

