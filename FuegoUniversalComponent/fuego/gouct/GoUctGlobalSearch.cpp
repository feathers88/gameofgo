//----------------------------------------------------------------------------
/** @file GoUctGlobalSearch.cpp */
//----------------------------------------------------------------------------

#include "../smartgame/SgSystem.h"
#include "../gouct/GoUctGlobalSearch.h"

//----------------------------------------------------------------------------

GoUctGlobalSearchStateParam::GoUctGlobalSearchStateParam()
    : m_mercyRule(true),
      m_territoryStatistics(false),
      m_lengthModification(0),
      m_scoreModification(0.02f),
      m_useTreeFilter(true)
{ }

GoUctGlobalSearchStateParam::~GoUctGlobalSearchStateParam()
{ }

//----------------------------------------------------------------------------
