import React from 'react';
import { useForm, SubmitHandler } from "react-hook-form";
import {List, ListItem} from '@mui/material';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

enum GenderEnum {
  female = "female",
  male = "male",
  other = "other"
}

interface IFormInput {
  firstName: String;
  gender: GenderEnum;
}

const Home2 = () => {
  const { register, handleSubmit } = useForm<IFormInput>();
  const onSubmit: SubmitHandler<IFormInput> = (data) => {
    console.log(data);
  }
  
  return (
    <ResponsiveDrawer>
      <div>
        <form onSubmit={handleSubmit(onSubmit)}>
          <List>
            <ListItem>
              <label>First Name</label>
              <input {...register("firstName")} />
            </ListItem><ListItem>
              <label>Gender Selection</label>
                <select {...register("gender")} >
                  <option value="female">female</option>
                  <option value="male">male</option>
                  <option value="other">other</option>
                </select>
            </ListItem>
            <ListItem>
              <input type="submit" />
            </ListItem>
          </List>
        </form>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home2;