using System.Collections.Generic;

namespace ColorCode.WinUITests
{
    /// <summary>
    /// Embedded snippets, one per language, each touching the scopes its language defines.
    /// </summary>
    public static class Samples
    {
        public sealed class Sample
        {
            public Sample(string name, ILanguage language, string code)
            {
                Name = name;
                Language = language;
                // Raw string literals take the source file's line endings, which depend on the checkout.
                Code = code.Replace("\r\n", "\n");
            }

            public string Name { get; }
            public ILanguage Language { get; }
            public string Code { get; }

            public override string ToString() => Name;
        }

        public static IReadOnlyList<Sample> All { get; } = new[]
        {
            new Sample("C#", Languages.CSharp, """
                // Copyright header comment
                #region Usings
                using System;
                #endregion

                namespace Demo
                {
                    /// <summary>A documented <see cref="Greeter"/>.</summary>
                    [Serializable]
                    public sealed class Greeter<T> where T : class
                    {
                        private const int Count = 42;

                        public string Greet(string name)
                        {
                            var message = $"Hello, {name}!";
                            /* block comment */
                            return Count > 0 ? message : @"verbatim \ string";
                        }
                    }
                }
                """),

            new Sample("XML", Languages.Xml, """
                <?xml version="1.0" encoding="utf-8"?>
                <!-- A comment -->
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup Condition="'$(Configuration)' == 'Release'">
                    <TargetFramework>net10.0</TargetFramework>
                    <Description><![CDATA[Some <raw> text]]></Description>
                  </PropertyGroup>
                  <ItemGroup>
                    <PackageReference Include="Uno.WinUI" Version="7.0.0" />
                  </ItemGroup>
                </Project>
                """),

            new Sample("HTML", Languages.Html, """
                <!DOCTYPE html>
                <html lang="en">
                <head>
                  <title>ColorCode</title>
                  <style>body { color: #333; }</style>
                  <script>console.log("hi");</script>
                </head>
                <body class="main">
                  <!-- comment -->
                  <p id="intro">Hello &amp; welcome</p>
                </body>
                </html>
                """),

            new Sample("JavaScript", Languages.JavaScript, """
                // Fetch and render
                import { render } from "./view.js";

                async function load(url, retries = 3) {
                  /* block comment */
                  const response = await fetch(url);
                  if (!response.ok && retries > 0) {
                    return load(url, retries - 1);
                  }
                  return render(await response.json(), 'single');
                }
                """),

            new Sample("TypeScript", Languages.Typescript, """
                interface Point { x: number; y: number; }

                export class Shape implements Drawable {
                  private readonly points: Point[] = [];

                  constructor(public name: string) {}

                  // Area of the polygon
                  area(): number {
                    return this.points.length > 2 ? 1.5 : 0;
                  }
                }
                """),

            new Sample("SQL", Languages.Sql, """
                -- Top customers
                SELECT TOP 10 c.Name, SUM(o.Total) AS Revenue
                FROM dbo.Customers AS c
                INNER JOIN dbo.Orders o ON o.CustomerId = c.Id
                WHERE o.CreatedAt >= '2026-01-01'
                  AND c.IsActive = 1
                GROUP BY c.Name
                ORDER BY Revenue DESC;
                /* block comment */
                """),

            new Sample("PowerShell", Languages.PowerShell, """
                # Build every project
                param([string]$Configuration = "Release")

                $projects = Get-ChildItem -Recurse -Filter *.csproj
                foreach ($project in $projects) {
                    Write-Host "Building $($project.Name)" -ForegroundColor Cyan
                    dotnet build $project.FullName -c $Configuration
                    if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
                }
                """),

            new Sample("CSS", Languages.Css, """
                /* Theme */
                :root {
                  --accent: #512BD4;
                }

                body > .card:hover,
                #main a[href^="https"] {
                  color: var(--accent);
                  margin: 0 auto !important;
                  font-family: "Segoe UI", sans-serif;
                }
                """),

            new Sample("C++", Languages.Cpp, """
                #include <iostream>
                #define SQUARE(x) ((x) * (x))

                // Entry point
                template <typename T>
                class Box {
                public:
                    explicit Box(T value) : value_(value) {}
                    T get() const { return value_; }
                private:
                    T value_;
                };

                int main() {
                    std::cout << "Area: " << SQUARE(3) << std::endl;
                    return 0;
                }
                """),

            new Sample("Java", Languages.Java, """
                package demo;

                import java.util.List;

                /** Javadoc comment */
                @FunctionalInterface
                public interface Visitor<T> {
                    // Line comment
                    void visit(T item);

                    static int count(List<String> items) {
                        return items == null ? 0 : items.size();
                    }
                }
                """),

            new Sample("Markdown", Languages.FindById("markdown"), """
                # Heading 1
                ## Heading 2

                Some **bold**, _emphasized_ and `inline code` text.

                - List item with a [link](https://platform.uno)
                - Another item

                > A block quote

                ```csharp
                var x = 1;
                ```
                """),

            new Sample("Haskell", Languages.Haskell, """
                -- Type variables render in italics
                module Main where

                data Tree a = Leaf | Node (Tree a) a (Tree a)

                depth :: Tree a -> Int
                depth Leaf = 0
                depth (Node l _ r) = 1 + max (depth l) (depth r)

                main :: IO ()
                main = print (depth (Node Leaf 'x' Leaf))
                """),

            new Sample("VB.NET", Languages.VbDotNet, """
                ' Line comment
                Imports System

                Public Class Greeter
                    Private Const Count As Integer = 42

                    Public Function Greet(name As String) As String
                        Return "Hello, " & name
                    End Function
                End Class
                """),
        };
    }
}
