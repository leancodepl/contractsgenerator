#!/bin/sh

set -e

if ! command -v dotnet >/dev/null 2>&1
then
    echo 1>&2 'dotnet: command not found'
    echo 1>&2
    echo 1>&2 'Acquire .NET SDK from https://dotnet.microsoft.com/download/dotnet'
    echo 1>&2 'or your favorite package manager and make sure that `dotnet` command is in your $PATH.'
    exit 1
fi

if ! dotnet --version >/dev/null
then
    exit 2
fi

# Exit codes? What's that?
restore="$(LC_ALL=C dotnet tool restore)"
case "${restore}" in
    (*"'dotnet-contracts-generate'"*'Restore was successful.'*)
    ;;
    (*'Restore was successful.'*)
        dotnet tool install dotnet-contracts-generate \
            ${SERVER_VERSION:+--version "${SERVER_VERSION}"}
    ;;
    (*'Cannot find a manifest file'*)
        dotnet tool install dotnet-contracts-generate \
            --create-manifest-if-needed \
            ${SERVER_VERSION:+--version "${SERVER_VERSION}"}
    ;;
    (*)
        echo 1>&2 "${restore}"
        exit 3
    ;;
esac

exec dotnet tool run dotnet-contracts-generate -- "${@}"
