"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const tl = require("vsts-task-lib/task");
function run() {
    return __awaiter(this, void 0, void 0, function* () {
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
            const result = yield dotnet.exec({
                cwd: __dirname
            });
            if (result > 0) {
                tl.setResult(tl.TaskResult.Failed, "Smoke Test failed");
                tl.error('Run failed');
            }
        }
        catch (err) {
            tl.setResult(tl.TaskResult.Failed, "Smoke Test failed");
            tl.error(err);
        }
    });
}
run().catch((reason) => tl.setResult(tl.TaskResult.Failed, reason));
;
