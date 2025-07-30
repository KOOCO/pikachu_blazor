# MCP Configuration for Noodle Project

## Atlassian MCP Setup

To enable Atlassian integration for all team members, run the following command:

```bash
claude mcp add --transport sse atlassian https://mcp.atlassian.com/v1/sse
```

This will provide access to:
- Confluence page management
- Jira issue tracking  
- Project documentation
- Task management

## Benefits for the Team

- **Confluence Integration**: Read/write project documentation directly
- **Jira Integration**: Create, update, and track issues
- **Unified Workflow**: Work with Atlassian tools without leaving Claude

## Usage Examples

Once connected, you can:
- Read Confluence pages: "Show me the API documentation from Confluence"
- Create Jira tickets: "Create a bug ticket for the wallet balance issue"
- Update documentation: "Update the deployment guide in Confluence"

## Project-Specific Resources

- Confluence Space: AT (Atlassian Tools)
- Project URL: https://kooco.atlassian.net