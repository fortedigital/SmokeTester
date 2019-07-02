import tl = require("vsts-task-lib/task");
import {IExecOptions} from "vsts-task-lib/toolrunner";

async function run() {
    var dotnetPath = tl.which("dotnet", true);
    var dotnet = tl.tool(dotnetPath);
    
    dotnet.arg("Forte.SmokeTester.dll");
    dotnet.arg("-u");
    dotnet.arg(tl.getInput("startUrl"));
    dotnet.arg("-d");
    dotnet.arg(tl.getInput("maxDepth"));
    dotnet.arg("--maxUrls");
    dotnet.arg(tl.getInput("maxUrls"));
    dotnet.arg("--maxErrors");
    dotnet.arg(tl.getInput("maxErrors"));
    dotnet.arg("-w");
    dotnet.arg(tl.getInput("numberOfWorkers"));
	dotnet.arg("-r");
    dotnet.arg(tl.getInput("retries"));
		
    if (tl.getInput("httpHeaders")) {
        dotnet.arg("-h");
        dotnet.arg(tl.getInput("httpHeaders"));
    }
	
	if (tl.getInput("minUrls")) {
        dotnet.arg("--minUrls");
        dotnet.arg(tl.getInput("minUrls"));
    }
	
	if (tl.getInput("requestTimeout")) {
        dotnet.arg("--timeout");
        dotnet.arg(tl.getInput("requestTimeout"));
    }
    
    try {
        const result = await dotnet.exec(<IExecOptions>{
            cwd: __dirname
        });

        if (result > 0){
            tl.setResult(tl.TaskResult.Failed, "Smoke Test failed")
            tl.error('Run failed');
        }
    } catch(err){
		tl.setResult(tl.TaskResult.Failed, "Smoke Test failed")
        tl.error(err);
    }
}

run().catch((reason) => tl.setResult(tl.TaskResult.Failed, reason));;