namespace Management.core
{
    //These are the common attributes of the entity of hospital staff
    [System.Serializable]
    public class Entity
    {
        //public float experience = 1f;//Range(0.1 - 10)
        //public float salary = 1f;//Range(0.1 - 10)
        //public float skills = 1f;//Range(0.1 - 10)

        /// <summary>
        /// Get entity value based on current values of experience, salary, skills
        /// </summary>
        /// <returns></returns>
        //public virtual float GetEntityWorkTime()
        //{
        //    /*Example:
        //     Doctor experience 10(Max)
        //     Doctor Skills 10 (Max)
        //     Doctor Salary 10 (Max)
        //     time = experience + skills + salary (10 + 10 + 10)
        //     time = 30;
        //    */
        //    return experience + skills + salary;
        //}

        /// <summary>
        /// Get Entity work value out of global values
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        //public virtual float GetWorkTime(float maxValue)
        //{
        //    float workTime = GetEntityWorkTime();
        //    return maxValue - workTime;
        //}

        /// <summary>
        /// Adds the parameter salary to the base salary
        /// </summary>
        /// <param name="upgradeSalaryBy"></param>
        //public void UpgradeSalary(float upgradeSalaryBy, float maxSalary)
        //{
        //    salary += upgradeSalaryBy;
        //    if (salary >= maxSalary)
        //        salary = maxSalary;
        //}

        /// <summary>
        /// Returns true if salary can upgrade, false if salry already upgraded to max value
        /// </summary>
        /// <param name="maxSalary"></param>
        /// <returns></returns>
        //public bool DoesSalaryCanUpgrade(float maxSalary)
        //{
        //    return salary < maxSalary;
        //}

        /// <summary>
        /// Adds the parameter experience to the base experience
        /// </summary>
        /// <param name="upgradeExperienceBy"></param>
        //public void UpgradeExperience(float upgradeExperienceBy, float maxExperience)
        //{
        //    experience += upgradeExperienceBy;
        //    if (experience >= maxExperience)
        //        experience = maxExperience;
        //}

        /// <summary>
        /// Adds the parameter skills to the base skill
        /// </summary>
        /// <param name="upgradeSkillsBy"></param>
        //public void UpgradeSkills(float upgradeSkillsBy, float maxSkills)
        //{
        //    skills += upgradeSkillsBy;
        //    if (skills >= maxSkills)
        //        skills = maxSkills;
        //}
    }
}