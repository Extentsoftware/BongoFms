#!/bin/bash

__start_indexing() {
	echo "*** Key is : ${apiFunctionKey} ***"
	sed -i -e 's/apiFunctionsKey/'"$apiFunctionKey"'/g' /etc/secrets/host.json
	dotnet Bongo.Functions.dll
}

__main() {
__start_indexing
}

__main "$@"
exit "$?"
