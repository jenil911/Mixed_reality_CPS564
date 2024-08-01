# Access Control Configuration

## Create Asset Control configurations

To create an Asset Control configuration, right-click on **Project Window** and select **Create** > **Services** > **Access Control Configuration**..

## Configuration Format
Configurations must conform to the [JSON schema] (https://ugs-config-schemas.unity3d.com/v1/project-access-policy.schema.json) for it to parse properly. 

### Example
The following example of configuration contains one statement that doesn't allow players to execute any Cloud Code module:

```json
{
  "$schema": "https://ugs-config-schemas.unity3d.com/v1/project-access-policy.schema.json",
  "Statements": [
    {
      "Sid": "deny-cloud-code-access",
      "Action": [
        "*"
      ],
      "Effect": "Deny",
      "Principal": "Player",
      "Resource": "urn:ugs:cloud-code:/v1/projects/*/modules/*",
      "Version": "1.0.0"
    }
  ]
}
```

## File Deployment
After you create the configurations, you can deploy them to the environment.
To deploy a file, go to **Window** > **Deployment** (2021.3-) or **Services** > **Deployment** (2022+).  
The Deployment window opens and displays all of your local Access Control configurations and enables you to deploy them.
For more information on the expected deployment window workflow, refer to ["com.unity.services.deployment"](https://docs.unity3d.com/Packages/com.unity.services.deployment@latest).
