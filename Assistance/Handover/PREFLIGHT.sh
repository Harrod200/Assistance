#!/usr/bin/env bash
# CreepingBorders handover preflight — run this FIRST, every session.
# Goal: cold start -> green build in under 5 minutes, no exploration needed.
set -e
cd "$(dirname "$0")"

echo "== 1/5 .NET SDK =="
if [ -x "$HOME/dotnet/dotnet" ]; then
  echo "   OK (already installed)"
else
  wget -q https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
  bash dotnet-install.sh --channel 8.0 --install-dir "$HOME/dotnet" >/dev/null
  echo "   installed"
fi
export PATH="$HOME/dotnet:$PATH"

echo "== 2/5 NuGet reference assemblies =="
if [ -d "$HOME/.nuget/packages/microsoft.netframework.referenceassemblies.net48" ]; then
  echo "   OK (cached)"
else
  mkdir -p "$HOME/.nuget/packages"
  cp -r ref-dlls/nuget-cache/* "$HOME/.nuget/packages/"
  echo "   restored from ref-dlls/nuget-cache"
fi
export FrameworkPathOverride="$HOME/.nuget/packages/microsoft.netframework.referenceassemblies.net48/1.0.3/build/.NETFramework/v4.8/"

echo "== 3/5 code build =="
dotnet build -v q 2>&1 | tail -3

echo "== 4/5 runtime caches present =="
for f in "Cached Data/PolygonCache.csv" "Cached Data/BorderDistanceCache.csv"; do
  [ -f "$f" ] && echo "   OK $f" || echo "   MISSING $f"
done

echo "== 5/5 sanity: manager compiles into build =="
if find bin -name "PolygonalRegionConnectivityManager.dll" | grep -q .; then
  echo "   OK"
else
  echo "   (check build output above)"
fi

echo "PREFLIGHT DONE. Next: read Docs/Handover Template.md"
