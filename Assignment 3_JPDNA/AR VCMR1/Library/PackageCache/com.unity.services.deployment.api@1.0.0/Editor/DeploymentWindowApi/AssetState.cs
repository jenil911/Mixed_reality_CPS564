using System;

namespace Unity.Services.DeploymentApi.Editor
{
    /// <summary>
    /// A struct that represents the state of a deployable asset locally.
    /// This should be used to represent the validity of the item itself,
    /// such as format and naming.
    /// For status relative to the 'live' counterpart, use DeploymentStatus
    /// </summary>
    [Serializable]
    public struct AssetState
    {
        //n.b.: Do not mark as readonly. Required for reload domain persistence.
        string m_Description;
        string m_Detail;
        SeverityLevel m_SeverityLevel;

        /// <summary>
        /// Description of the state.
        /// </summary>
        public string Description => m_Description;

        /// <summary>
        /// A detailed description of the state.
        /// Displayed in the Status Panel of the Deployment Window on deployment item selection.
        /// </summary>
        public string Detail => m_Detail;

        /// <summary>
        /// The level associated with this state.
        /// </summary>
        public SeverityLevel Level => m_SeverityLevel;

        /// <summary>
        /// Constructor of the AssetState.
        /// </summary>
        /// <param name="description">Status description.</param>
        /// <param name="detail">Detail.</param>
        /// <param name="level">Deployment status type.</param>
        public AssetState(string description, string detail, SeverityLevel level)
        {
            m_Description = description;
            m_Detail = detail;
            m_SeverityLevel = level;
        }
    }
}
