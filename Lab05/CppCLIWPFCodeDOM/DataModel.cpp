#include "DataModel.h"
#include <msclr\marshal_cppstd.h>

using namespace System::IO;
using namespace System::Linq;
using namespace System::Collections::Generic;
using namespace System::Reflection;
using namespace System::Text;
using namespace Microsoft::CodeAnalysis;

// Dodaj referencj? do DLL RoslynCompiler
#using "..\RoslynCompiler\bin\Debug\net9.0\RoslynCompiler.dll"

using namespace RoslynCompiler;

DataModel::DataModel(String^ codeText)
{
	_codeText = codeText;
}

DataModel::NavigateToAddCodeCommand::NavigateToAddCodeCommand(DataModel^ viewModel)
{
    _viewModel = viewModel;
}

bool DataModel::NavigateToAddCodeCommand::CanExecute(System::Object^ parameter) 
{
    return true;
}

void DataModel::NavigateToAddCodeCommand::Execute(System::Object^ parameter)
{
    _viewModel->ErrorsList->Clear();
    _viewModel->MethodsList->Clear();
    _viewModel->FieldsList->Clear();
    
    try {
        auto compilationResult = VBCompiler::CompileVBCode(
            _viewModel->CodeText, 
            (Microsoft::CodeAnalysis::OutputKind)_viewModel->SelectedOutputKind
        );
        
        if (!compilationResult->Success)
        {
            for each (String^ error in compilationResult->Errors)
            {
                _viewModel->ErrorsList->Add(error);
            }
        }
        else
        {
            auto assembly = Assembly::Load(compilationResult->AssemblyBytes);

            auto allTypes = assembly->GetTypes();
            
            
            auto types = assembly->GetExportedTypes();
            
            if (types->Length > 0 || allTypes->Length > 0)
            {
                _viewModel->Type = (types->Length > 0) ? types[0] : allTypes[0];
                
                _viewModel->ObjectInstance = Activator::CreateInstance(_viewModel->Type);
                
                auto allMethods = _viewModel->Type->GetMethods(
                    BindingFlags::Public | BindingFlags::Instance | BindingFlags::DeclaredOnly
                );
                _viewModel->Methods = allMethods;
                
                auto allFields = _viewModel->Type->GetFields(
                    BindingFlags::Public | BindingFlags::Instance
                );
                _viewModel->Fields = allFields;
                _viewModel->ErrorsList->Add("Done");
            }
            else
            {
            }
        }
    }
    catch (Exception^ ex)
    {
        _viewModel->ErrorsList->Add("Exception: " + ex->Message);
        if (ex->InnerException != nullptr) {
            _viewModel->ErrorsList->Add("Inner: " + ex->InnerException->Message);
        }
        _viewModel->ErrorsList->Add("Stack: " + ex->StackTrace);
    }
}

void DataModel::UpdateMethodsList()
{
    MethodsList->Clear();
    if (methods != nullptr)
    {
        for each (MethodInfo^ method in methods)
        {
            StringBuilder^ sb = gcnew StringBuilder();
            sb->Append(method->Name);
            
            MethodsList->Add(sb->ToString());
        }
    }
}

void DataModel::UpdateFieldsList()
{
    FieldsList->Clear();
    if (fields != nullptr)
    {
        for each (FieldInfo^ field in fields)
        {
            FieldsList->Add(field->Name);
        }
    }
}

void DataModel::UpdateFieldInfo()
{
    if (fields != nullptr && _selectedField != nullptr)
    {
        for each (FieldInfo^ field in fields)
        {
            String^ fieldStr = field->Name;
            if (fieldStr == _selectedField)
            {
                try {
                    FieldType = field->FieldType->Name;
                    Object^ value = field->GetValue(ObjectInstance);
                    FieldValue = (value != nullptr) ? value->ToString() : "null";
                }
                catch (Exception^ ex)
                {
                    FieldValue = "Error: " + ex->Message;
                }
                break;
            }
        }
    }
}

DataModel::InvokeMethodCommandClass::InvokeMethodCommandClass(DataModel^ viewModel)
{
    _viewModel = viewModel;
}

bool DataModel::InvokeMethodCommandClass::CanExecute(System::Object^ parameter)
{
    return true;
}

void DataModel::InvokeMethodCommandClass::Execute(System::Object^ parameter)
{
    if (_viewModel->methods == nullptr || _viewModel->SelectedMethod == nullptr)
    {
        _viewModel->MethodResult = "No method selected";
        return;
    }
    
    try {
        MethodInfo^ selectedMethodInfo = nullptr;
        for each (MethodInfo^ method in _viewModel->methods)
        {
            StringBuilder^ sb = gcnew StringBuilder();
            sb->Append(method->Name);
            
            if (sb->ToString() == _viewModel->SelectedMethod)
            {
                selectedMethodInfo = method;
                break;
            }
        }
        
        if (selectedMethodInfo != nullptr)
        {
            auto params = selectedMethodInfo->GetParameters();
            array<Object^>^ args = nullptr;
            
            if (params->Length > 0)
            {
                args = gcnew array<Object^>(params->Length);
                
                if (!String::IsNullOrEmpty(_viewModel->MethodParameter))
                {
                    args[0] = _viewModel->MethodParameter;
                }
            }
            
            Object^ result = selectedMethodInfo->Invoke(_viewModel->ObjectInstance, args);
            _viewModel->MethodResult = (result != nullptr) ? result->ToString() : "null";
        }
        else
        {
            _viewModel->MethodResult = "Method not found";
        }
    }
    catch (Exception^ ex)
    {
        _viewModel->MethodResult = "Error: " + ex->Message;
    }
}

DataModel::SetFieldCommandClass::SetFieldCommandClass(DataModel^ viewModel)
{
    _viewModel = viewModel;
}

bool DataModel::SetFieldCommandClass::CanExecute(System::Object^ parameter)
{
    return true;
}

void DataModel::SetFieldCommandClass::Execute(System::Object^ parameter)
{
    if (_viewModel->fields == nullptr || _viewModel->SelectedField == nullptr)
    {
        return;
    }
    
    try {
        for each (FieldInfo^ field in _viewModel->fields)
        {
            String^ fieldStr = field->Name;
            if (fieldStr == _viewModel->SelectedField)
            {
                Object^ value = nullptr;
                
                if (field->FieldType == String::typeid)
                {
                    value = _viewModel->FieldValue;
                }
                else if (field->FieldType == int::typeid || field->FieldType == Int32::typeid)
                {
                    value = Int32::Parse(_viewModel->FieldValue);
                }
                else if (field->FieldType == bool::typeid || field->FieldType == Boolean::typeid)
                {
                    value = Boolean::Parse(_viewModel->FieldValue);
                }
                else if (field->FieldType == double::typeid || field->FieldType == Double::typeid)
                {
                    value = Double::Parse(_viewModel->FieldValue);
                }
                
                field->SetValue(_viewModel->ObjectInstance, value);
                
                _viewModel->UpdateFieldInfo();
                break;
            }
        }
    }
    catch (Exception^ ex)
    {
        _viewModel->FieldValue = "Error: " + ex->Message;
    }
}


