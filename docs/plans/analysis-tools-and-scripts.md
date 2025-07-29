# Analysis Tools and Scripts for Migration Study

## Overview

This document provides specific tools, scripts, and techniques to efficiently analyze the Sonarr → Lidarr → Readarr migration patterns.

## Git Analysis Scripts

### 1. Fork Point Verification Script

```bash
#!/bin/bash
# verify-fork-points.sh

echo "=== Verifying Sonarr → Lidarr Fork Point ==="
cd sonarr
SONARR_COMMIT=$(git log --format="%H" -n 1 83370dd 2>/dev/null)
if [ -n "$SONARR_COMMIT" ]; then
    echo "Found Sonarr commit: $SONARR_COMMIT"
    git log --oneline -1 $SONARR_COMMIT
    echo "Date: $(git log --format="%ai" -1 $SONARR_COMMIT)"
else
    echo "ERROR: Could not find Sonarr commit 83370dd"
fi

echo -e "\n=== Verifying Lidarr → Readarr Fork Point ==="
cd ../lidarr
LIDARR_COMMIT=$(git log --format="%H" -n 1 47f4441 2>/dev/null)
if [ -n "$LIDARR_COMMIT" ]; then
    echo "Found Lidarr commit: $LIDARR_COMMIT"
    git log --oneline -1 $LIDARR_COMMIT
    echo "Date: $(git log --format="%ai" -1 $LIDARR_COMMIT)"
else
    echo "ERROR: Could not find Lidarr commit 47f4441"
fi
```

### 2. File Structure Comparison Script

```bash
#!/bin/bash
# compare-structure.sh

echo "=== Comparing Project Structures ==="

for project in sonarr lidarr readarr; do
    echo -e "\n--- $project structure ---"
    cd $project
    find . -name "*.csproj" -type f | grep -E "(Core|Api|Host)" | sort > ../${project}_projects.txt
    find . -type d -name "Controllers" -o -name "Services" -o -name "Models" | grep -v "/obj/" | grep -v "/bin/" | sort > ../${project}_dirs.txt
    cd ..
done

echo -e "\n=== Key Differences ==="
echo "Projects unique to Lidarr vs Sonarr:"
comm -13 sonarr_projects.txt lidarr_projects.txt

echo -e "\nProjects unique to Readarr vs Lidarr:"
comm -13 lidarr_projects.txt readarr_projects.txt
```

### 3. Domain Model Extraction Script

```bash
#!/bin/bash
# extract-domain-models.sh

extract_models() {
    local project=$1
    local namespace=$2
    
    echo "=== Extracting models from $project ==="
    cd $project
    
    # Find core entity files
    find . -path "*/bin" -prune -o -path "*/obj" -prune -o -name "*.cs" -type f -exec grep -l "class.*${namespace}" {} \; | grep -v Test > ../${project}_models.txt
    
    # Extract class definitions
    for file in $(cat ../${project}_models.txt); do
        echo -e "\n--- $file ---"
        grep -A 5 "public class" "$file" | grep -v "^--$"
    done > ../${project}_class_definitions.txt
    
    cd ..
}

extract_models "sonarr" "Series\|Season\|Episode"
extract_models "lidarr" "Artist\|Album\|Track"
extract_models "readarr" "Author\|Book\|Edition"
```

## Code Analysis Tools

### 1. Namespace Migration Analyzer

```python
#!/usr/bin/env python3
# namespace_analyzer.py

import os
import re
from collections import defaultdict

def analyze_namespaces(project_path, old_pattern, new_pattern):
    namespace_changes = defaultdict(list)
    
    for root, dirs, files in os.walk(project_path):
        # Skip build directories
        if '/bin/' in root or '/obj/' in root:
            continue
            
        for file in files:
            if file.endswith('.cs'):
                filepath = os.path.join(root, file)
                with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
                    content = f.read()
                    
                # Find namespace declarations
                old_matches = re.findall(f'namespace.*{old_pattern}.*', content)
                new_matches = re.findall(f'namespace.*{new_pattern}.*', content)
                
                if old_matches or new_matches:
                    namespace_changes[filepath].extend(old_matches + new_matches)
    
    return namespace_changes

# Usage
sonarr_ns = analyze_namespaces('sonarr', 'Tv', 'Series')
lidarr_ns = analyze_namespaces('lidarr', 'Music', 'Artist')
readarr_ns = analyze_namespaces('readarr', 'Books', 'Author')
```

### 2. API Endpoint Comparison Tool

```python
#!/usr/bin/env python3
# api_endpoint_analyzer.py

import os
import re
from collections import defaultdict

def extract_api_endpoints(project_path):
    endpoints = defaultdict(list)
    
    for root, dirs, files in os.walk(project_path):
        if '/bin/' in root or '/obj/' in root:
            continue
            
        for file in files:
            if file.endswith('Controller.cs'):
                filepath = os.path.join(root, file)
                with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
                    content = f.read()
                    
                # Extract HTTP method attributes and routes
                methods = re.findall(r'\[(HttpGet|HttpPost|HttpPut|HttpDelete|Route)\("?([^"\]]*)"?\)\]', content)
                controller_name = file.replace('Controller.cs', '')
                
                for method, route in methods:
                    endpoints[controller_name].append((method, route))
    
    return endpoints

# Compare endpoints across projects
sonarr_endpoints = extract_api_endpoints('sonarr')
lidarr_endpoints = extract_api_endpoints('lidarr')
readarr_endpoints = extract_api_endpoints('readarr')
```

### 3. Database Schema Extractor

```python
#!/usr/bin/env python3
# schema_extractor.py

import os
import re
from collections import defaultdict

def extract_entity_properties(project_path, entity_names):
    entities = defaultdict(dict)
    
    for root, dirs, files in os.walk(project_path):
        if '/bin/' in root or '/obj/' in root:
            continue
            
        for file in files:
            if file.endswith('.cs'):
                filepath = os.path.join(root, file)
                with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
                    content = f.read()
                    
                for entity in entity_names:
                    if f'class {entity}' in content:
                        # Extract properties
                        props = re.findall(r'public\s+(\w+\??)\s+(\w+)\s*\{', content)
                        entities[entity] = props
    
    return entities

# Extract core entities
sonarr_entities = extract_entity_properties('sonarr', ['Series', 'Season', 'Episode'])
lidarr_entities = extract_entity_properties('lidarr', ['Artist', 'Album', 'Track'])
readarr_entities = extract_entity_properties('readarr', ['Author', 'Book', 'Edition'])
```

## Visualization Scripts

### 1. Entity Relationship Diagram Generator

```python
#!/usr/bin/env python3
# generate_erd.py

def generate_plantuml_erd(entities, project_name):
    uml = f"@startuml {project_name}_erd\n"
    uml += f"title {project_name} Core Entities\n\n"
    
    for entity, properties in entities.items():
        uml += f"class {entity} {{\n"
        for prop_type, prop_name in properties[:10]:  # Limit to first 10 properties
            uml += f"  +{prop_name}: {prop_type}\n"
        uml += "}\n\n"
    
    # Add basic relationships (customize based on actual relationships)
    if project_name == "Sonarr":
        uml += "Series \"1\" --> \"*\" Season\n"
        uml += "Season \"1\" --> \"*\" Episode\n"
    elif project_name == "Lidarr":
        uml += "Artist \"1\" --> \"*\" Album\n"
        uml += "Album \"1\" --> \"*\" Track\n"
    elif project_name == "Readarr":
        uml += "Author \"1\" --> \"*\" Book\n"
        uml += "Book \"1\" --> \"*\" Edition\n"
    
    uml += "@enduml"
    return uml
```

### 2. Migration Timeline Visualizer

```python
#!/usr/bin/env python3
# migration_timeline.py

import subprocess
from datetime import datetime

def get_commit_info(repo_path, commit_hash):
    cmd = f"cd {repo_path} && git log --format='%ai|%an|%s' -1 {commit_hash}"
    result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
    if result.returncode == 0:
        parts = result.stdout.strip().split('|')
        return {
            'date': parts[0],
            'author': parts[1],
            'message': parts[2]
        }
    return None

# Generate timeline
timeline = []
timeline.append(('Sonarr', '83370dd', get_commit_info('sonarr', '83370dd')))
timeline.append(('Lidarr', '47f4441', get_commit_info('lidarr', '47f4441')))

print("=== Migration Timeline ===")
for project, commit, info in timeline:
    if info:
        print(f"\n{project} Fork Point:")
        print(f"  Commit: {commit}")
        print(f"  Date: {info['date']}")
        print(f"  Author: {info['author']}")
        print(f"  Message: {info['message']}")
```

## Automated Comparison Reports

### 1. Generate Comprehensive Comparison Report

```bash
#!/bin/bash
# generate-comparison-report.sh

REPORT_DIR="docs/analysis/automated-reports"
mkdir -p $REPORT_DIR

echo "# Automated Migration Analysis Report" > $REPORT_DIR/comparison-report.md
echo "Generated on: $(date)" >> $REPORT_DIR/comparison-report.md

# File count comparison
echo -e "\n## File Statistics" >> $REPORT_DIR/comparison-report.md
for project in sonarr lidarr readarr; do
    cs_count=$(find $project -name "*.cs" -type f | wc -l)
    echo "- $project: $cs_count C# files" >> $REPORT_DIR/comparison-report.md
done

# Line count comparison
echo -e "\n## Code Volume" >> $REPORT_DIR/comparison-report.md
for project in sonarr lidarr readarr; do
    loc=$(find $project -name "*.cs" -type f -exec cat {} \; | wc -l)
    echo "- $project: $loc lines of C# code" >> $REPORT_DIR/comparison-report.md
done

# Project structure
echo -e "\n## Project Structure" >> $REPORT_DIR/comparison-report.md
for project in sonarr lidarr readarr; do
    echo -e "\n### $project Projects" >> $REPORT_DIR/comparison-report.md
    find $project -name "*.csproj" -type f | sort >> $REPORT_DIR/comparison-report.md
done
```

### 2. Difference Highlighter

```python
#!/usr/bin/env python3
# highlight_differences.py

import difflib
import os

def compare_files(file1, file2, output_file):
    with open(file1, 'r') as f1, open(file2, 'r') as f2:
        lines1 = f1.readlines()
        lines2 = f2.readlines()
    
    differ = difflib.unified_diff(lines1, lines2, fromfile=file1, tofile=file2)
    
    with open(output_file, 'w') as out:
        out.writelines(differ)

# Compare key configuration files
compare_files('sonarr/src/config.json', 'lidarr/src/config.json', 'sonarr_lidarr_config_diff.txt')
compare_files('lidarr/src/config.json', 'readarr/src/config.json', 'lidarr_readarr_config_diff.txt')
```

## Integration Analysis Tools

### 1. Provider Detection Script

```bash
#!/bin/bash
# detect-providers.sh

echo "=== External Provider Analysis ==="

for project in sonarr lidarr readarr; do
    echo -e "\n--- $project providers ---"
    cd $project
    
    # Find metadata providers
    echo "Metadata Providers:"
    grep -r "IProvideSeriesInfo\|IProvideArtistInfo\|IProvideAuthorInfo" --include="*.cs" | cut -d: -f1 | sort -u
    
    # Find download clients
    echo -e "\nDownload Clients:"
    find . -name "*DownloadClient*.cs" -type f | grep -v Test | sort
    
    # Find indexers
    echo -e "\nIndexers:"
    find . -name "*Indexer*.cs" -type f | grep -v Test | sort
    
    cd ..
done
```

## Usage Instructions

1. **Setup**: Place all scripts in a `scripts/` directory at the repository root
2. **Make executable**: `chmod +x scripts/*.sh`
3. **Run analysis**: Execute scripts in order, starting with structure comparison
4. **Collect results**: Output will be in various formats - combine into final report
5. **Visualize**: Use PlantUML or similar tools to render generated diagrams

## Best Practices

1. **Version Control**: Commit analysis scripts to track changes
2. **Automation**: Set up CI/CD to run analysis on updates
3. **Documentation**: Document any assumptions made by scripts
4. **Validation**: Manually verify script outputs for accuracy
5. **Iteration**: Refine scripts based on initial findings

These tools and scripts provide a solid foundation for systematically analyzing the migration patterns between Sonarr, Lidarr, and Readarr.