# GenerateImageGallery.ps1
# This script scans all PNG files starting with "DotNetPerfBenchmark.ZipCodeBenchmarks-"
# in the bin\Release\net9.0\BenchmarkDotNet.Artifacts\results folder,
# groups them by category (parsed from the filename),
# and creates a Flexbox-based HTML gallery (index.html) to display them.

# Resolve the path to the BenchmarkDotNet artifacts 'results' folder
$resultsFolder = Join-Path $PSScriptRoot 'bin\Release\net9.0\BenchmarkDotNet.Artifacts\results'

# The output HTML will go in the same folder as the images
$outputFile = Join-Path $resultsFolder 'index.html'

# The prefix to identify your files
$prefix = "DotNetPerfBenchmark.ZipCodeBenchmarks-"

# Basic CSS with Flexbox. Customize as you wish.
$css = @"
<style>
body {
    font-family: Arial, sans-serif;
    margin: 20px;
    background: #f9f9f9;
}
h1 {
    font-size: 1.5em;
    margin-bottom: 1em;
}
.category {
    margin-bottom: 50px;
    border: 1px solid #ccc;
    background: #fff;
    padding: 15px;
}
.category h2 {
    margin-top: 0;
    color: #444;
}
/* Flex container for images in each category */
.flex-container {
    display: flex;
    flex-wrap: wrap;
    gap: 16px;            /* spacing between items */
    justify-content: flex-start;
}
/* Each image "card" */
.flex-item {
    flex: 1 1 300px;      /* grow/shrink, min width ~300px */
    max-width: 500px;     /* cap the width for very large screens */
    border: 1px solid #ccc;
    padding: 8px;
    background: #fafafa;
}
.flex-item img {
    display: block;
    width: 100%;
    height: auto;
    border: 1px solid #ccc;
    background: #fff;
}
</style>
"@

# HTML header and footer
$htmlHeader = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>BenchmarkDotNet PNG Gallery</title>
    $css
</head>
<body>
    <h1>BenchmarkDotNet: ZipCodeBenchmarks Image Gallery</h1>
"@

$htmlFooter = @"
</body>
</html>
"@

# Hashtable for category -> list of image filenames
$groups = @{}

# Collect relevant PNG files from the results folder
$pngFiles = Get-ChildItem -Path $resultsFolder -Filter *.png -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -like "$prefix*" }

if (-not $pngFiles) {
    Write-Host "No PNG files found in $resultsFolder matching '$prefix*'. Exiting."
    return
}

foreach ($file in $pngFiles) {
    $filename = $file.Name

    # Remove the prefix
    $rest = $filename.Substring($prefix.Length)

    # Extract category (the part up to the first dash, if any)
    $dashIndex = $rest.IndexOf("-")
    if ($dashIndex -ge 0) {
        $category = $rest.Substring(0, $dashIndex)
    } else {
        $category = $rest
    }

    # Initialize list if we haven't seen this category
    if (-not $groups.ContainsKey($category)) {
        $groups[$category] = New-Object System.Collections.Generic.List[string]
    }

    $groups[$category].Add($filename)
}

# Build the HTML for each category
$htmlBody = ""

foreach ($category in $groups.Keys) {
    $htmlBody += "<div class='category'>`n"
    $htmlBody += "  <h2>$category</h2>`n"
    $htmlBody += "  <div class='flex-container'>`n"

    foreach ($img in $groups[$category]) {
        $htmlBody += "    <div class='flex-item'>`n"
        $htmlBody += "      <img src='$img' alt='$img' />`n"
        $htmlBody += "    </div>`n"
    }

    $htmlBody += "  </div>`n"
    $htmlBody += "</div>`n"
}

# Combine header, body, and footer
$htmlContent = $htmlHeader + $htmlBody + $htmlFooter

# Write the output HTML
$htmlContent | Out-File $outputFile -Encoding UTF8

Write-Host "Gallery generated at: $outputFile"
Write-Host "Open $outputFile in your browser to see the Flexbox gallery."
