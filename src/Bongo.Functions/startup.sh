#!/bin/bash

__start_indexing() {
	echo "*** Key is : ${apiFunctionKey} ***"
	sed -i -e 's/apiFunctionsKey/'"$apiFunctionKey"'/g' /etc/secrets/host.json
	dotnet /azure-functions-host/Microsoft.Azure.WebJobs.Script.WebHost.dll
}

__main() {
__start_indexing
}

__main "$@"
exit "$?"
