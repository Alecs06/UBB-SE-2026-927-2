
# Setup
### SQL Server
1. Create a new Database in MSSM
2. Copy the connection string from MSSM
3. Add "Initial Catalog=TestsAndInterviews;" to the connection string
4. In the project in the Env.cs class change the connection string
5. In MSSM run the sql script in the SQL folder from the project
### From Imre's assignment
Use styleCop static code analysis to find issues in the code and fix them, this will also 
help to homogenize the various projects a bit so merging them will be less painful.
1. Copy the SE.ruleset file from teams to your project(s) root.
2. Right click on project(s)-> manage NugetPackages
3. On the Browse tab search for -> StyleCop.Analyzers, then install
4. Right click on your project -> Edit project file
5. Search for the following property group and add the line with 
"CodeAnalysisRuleSet", if it does not exist add all three. 
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
<CodeAnalysisRuleSet>SE.ruleset</CodeAnalysisRuleSet>
</PropertyGroup>
6. Build/attempt to run

How to use: 
build. look in the warnings and there should be a lot with SAxxxx, these are the stylecop warnings.
Fix them and build again until there are no more warnings. Also, they are underlined in the code, so you can easily find them.