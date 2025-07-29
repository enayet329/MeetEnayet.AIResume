# MeetEnayetAI

AI-powered interactive resume API using .NET 8, Semantic Kernel, and PDF embedding-based RAG. No chat history is stored between sessions.

## Features
- Stateless API with GPT-4
- Reads resume PDF for personality context
- Semantic Kernel 1.61.0 integration
- Swagger-enabled

## Setup
1. Add your OpenAI key to `.env`
2. Place your resume PDF at `./Docs/enayet_resume.pdf`
3. Run:
```bash
dotnet run
```
4. Test via Swagger at `http://localhost:5000/swagger`
