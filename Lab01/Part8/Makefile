.PHONY: build test clean publish run help

# Default target
all: build

# Build the project
build:
	@echo "Building project..."
	./build-dotnet.sh --target=Build

# Run full pipeline (default Cake target)
pipeline:
	@echo "Running full build pipeline..."
	./build-dotnet.sh

# Run tests
test:
	@echo "Running tests..."
	./build-dotnet.sh --target=Run-Unit-Tests

# Clean build artifacts
clean:
	@echo "Cleaning build artifacts..."
	./build-dotnet.sh --target=Clean

# Publish self-contained
publish:
	@echo "Publishing self-contained application..."
	./build-dotnet.sh --target=Publish

# Publish portable
publish-portable:
	@echo "Publishing portable application..."
	./build-dotnet.sh --target=Publish-Portable

# Run the published application
run:
	@if [ -f "./publish/OtwarteDane.Cli" ]; then \
		echo "Running published application..."; \
		./publish/OtwarteDane.Cli; \
	else \
		echo "Application not found. Run 'make publish' first."; \
		exit 1; \
	fi

# Development run with dotnet
dev-run:
	@echo "Running in development mode..."
	dotnet run --project OtwarteDane.Cli

# Development build and run
dev: 
	dotnet build && dotnet run --project OtwarteDane.Cli

# Show help
help:
	@echo "Available targets:"
	@echo "  build             - Build the project"
	@echo "  pipeline          - Run full build pipeline (default)"
	@echo "  test              - Run tests"
	@echo "  clean             - Clean build artifacts"
	@echo "  publish           - Publish self-contained application"
	@echo "  publish-portable  - Publish portable application"
	@echo "  run               - Run published application"
	@echo "  dev-run           - Run in development mode"
	@echo "  dev               - Quick development build and run"
	@echo "  help              - Show this help"