#!/bin/sh

set -e

repo='leancodepl/contractsgenerator'
name='LeanCode.ContractsGenerator'

if [ -z "${SERVER_VERSION}" ]
then
    tag="$(curl -s "https://api.github.com/repos/${repo}/releases/latest" \
        | grep -F '"tag_name":' \
        | cut -d'"' -f4)"

    SERVER_VERSION="${tag#v}"
fi

package="${name}.${SERVER_VERSION}.zip"
source="https://github.com/${repo}/releases/download/v${SERVER_VERSION}/${package}"
dest="${XDG_STATE_HOME:-${HOME}/.local/state}/${name}/${SERVER_VERSION}"

mkdir -p "${dest}"

if ! [ -f "${dest}/${name}.dll" ]
then
    dest_package="${dest}/${package}"
    curl -Lso "${dest_package}" "${source}"
    unzip -q "${dest_package}" -d "${dest}"
    rm -f "${dest_package}"
fi

if ! command -v dotnet >/dev/null 2>&1
then
    echo 1>&2 'dotnet: command not found'
    echo 1>&2
    echo 1>&2 "Acquire .NET runtime from https://dotnet.microsoft.com/download/dotnet"
    echo 1>&2 'or your favorite package manager and make sure that `dotnet` command is in your $PATH.'
    echo 1>&2
    echo 1>&2 "Min runtime version required: $(grep -F '"version":' "${dest}/${name}.runtimeconfig.json")"
    exit 1
fi

exec dotnet --roll-forward Major "${dest}/${name}.dll" "${@}"
