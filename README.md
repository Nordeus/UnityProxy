# Unity Proxy

Unity proxy is used to start Unity, redirect the log file to standard output and detect the magic string in the output that indicates if the build was successful or not. It also reports all progress bars as build progress to TeamCity. It can also copy the whole editor log after the build to the specified folder (artifactsPath). Command line arguments are as follows:
```sh
pathToUnityExecutable [-artifactsPath pathForBuildArtifacts]
```