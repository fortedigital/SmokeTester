# VSTS-Build-Task-Sample
Visual Studio Team Services Build Task Sample. This repo is to serve as a code sample for [http://www.andrewhoefling.com/](http://www.andrewhoefling.com/)

# Getting Started
Install the tfx-cli 
```npm i -g tfx-cli```

Package the extension using the tfx-cli
```tfx extension create --manifest-globs vss-extension.json```

Goto your publisher account [https://marketplace.visualstudio.com](https://marketplace.visualstudio.com) and upload the generated vsix