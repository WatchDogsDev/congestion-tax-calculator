# Congestion Tax Calculator

This repository contains a developer [assignment](ASSIGNMENT.md) used as a basis for candidate interview and evaluation.

## Project Status

I have completed both the main assignment and the bonus scenario. The congestion tax calculator has been fully implemented with proper architecture, comprehensive test coverage, and support for multiple cities with configurable tax rules.

## Implementation Overview

The solution implements a congestion tax calculator for Gothenburg with the following features:

- **Tax Calculation**: Implements all Gothenburg congestion tax rules including time-based rates, daily caps, and single charge rule
- **Multi-City Support**: Architecture supports multiple cities with different tax rules (bonus requirement)
- **Database Integration**: Uses Entity Framework Core with InMemory/SQLite for data persistence
- **Comprehensive Testing**: Full test coverage with unit tests validating all scenarios
- **Clean Architecture**: Separation of concerns with repositories, services, and domain models

## Folder Structure

```
netcore/
├── src/CongestionTaxCalculator/           # Main application
│   ├── Data/                             # Data access layer
│   │   ├── HolidayRepository/            # Holiday data management
│   │   ├── PassageRepository/            # Vehicle passage tracking
│   │   ├── TaxRuleRepository/            # Tax rule configuration
│   │   ├── VehicleRepository/            # Vehicle data management
│   │   └── TaxDbContext.cs              # Entity Framework context
│   ├── Enums/                            # Application enumerations
│   │   ├── Currency.cs                   # Currency types
│   │   └── VehicleType.cs               # Vehicle classifications
│   ├── Models/                           # Domain models
│   │   ├── City.cs                       # City configuration
│   │   ├── Holiday.cs                    # Holiday definitions
│   │   ├── IVehicle.cs                   # Vehicle interface
│   │   ├── Passage.cs                    # Vehicle passage records
│   │   ├── TaxRule.cs                    # Tax rule definitions
│   │   └── Vehicle.cs                    # Vehicle implementation
│   ├── Services/                         # Business logic services
│   │   ├── DayRuleCheckerService/        # Day eligibility checking
│   │   └── TaxCalculatorService/         # Core tax calculation
│   └── Types/                            # Value types
│       └── Money.cs                      # Money value object
├── tests/CongestionTaxCalculator.Tests/  # Unit tests
│   └── TaxCalculatorTests.cs            # Comprehensive test suite
└── CongestionTaxCalculator.sln           # Solution file
```

## Key Features Implemented

### Core Functionality
- Time-based tax calculation according to Gothenburg rules
- Daily fee cap enforcement (60 SEK maximum)
- Single charge rule implementation (60-minute window)
- Tax-exempt vehicle handling
- Weekend and holiday exclusions
- July month exemption
- Day-before-holiday exemption

### Architecture Benefits
- **Repository Pattern**: Clean data access abstraction
- **Service Layer**: Business logic separation
- **Dependency Injection**: Loose coupling and testability
- **Entity Framework**: Robust data persistence
- **Value Objects**: Type-safe money handling

### Test Coverage
- All assignment test cases pass
- Edge case validation
- Tax-exempt vehicle scenarios
- Holiday and weekend exclusions
- Single charge rule verification
- Daily cap enforcement

## Bonus Scenario Implementation

The bonus requirement for multi-city support has been fully implemented:

- **City Configuration**: `City` model supports different tax rules per city
- **Database-Driven Rules**: Tax rules stored in database for runtime configuration
- **Flexible Architecture**: Easy addition of new cities without code changes
- **Content Management Ready**: Tax rules can be updated by content editors through database

## Areas for Improvement (Ignored for Simplicity)

While the current implementation is functional and meets all requirements, several enhancements could be considered for a production system:

- **Caching**: Tax rules and holidays could be cached for better performance
- **Logging**: Comprehensive logging for audit trails and debugging
- **Validation**: Input validation and error handling could be more robust
- **API Layer**: REST API endpoints for external system integration
- **Configuration**: External configuration files for easier deployment
- **Performance**: Database query optimization for large datasets
- **Clean Architecture**: More strict adherence to Clean Architecture principles with clear domain boundaries
- **Documentation**: API documentation and deployment guides

These improvements were intentionally omitted to keep the solution focused on the core requirements while maintaining clean, readable code.

